using MatGPT.Data;
using MatGPT.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Cryptography;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationContext _context;
        
        public AuthController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerReq)
        {
            if (_context.Credentials.Any(u => u.Email == registerReq.Email))
            {
                return BadRequest("User already exists.");
            }

            // Generates a Salt and a Hash for the password which will be stored in the database instead of the real password
            byte[] salt = GenerateSalt();
            byte[] hash = GenerateHash(registerReq.Password, salt);

            var credential = new Credential
            {
                Email = registerReq.Email,
                PasswordHash = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt)
            };

            var user = new User
            {
                FirstName = registerReq.FirstName,
                LastName = registerReq.LastName,
                Credential = credential
            };

            // Creates a new user to the database, try/catch to send success/error
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return Ok("Successfully Registered as new User!");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed registering new User", Error = ex.Message });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginReq)
        {
            
            // Finds the user trying to login using Email as username
            var user = await _context.Users
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u =>
                    u.Credential.Email == loginReq.Email);

            // Checks if the user exists
            if (user == null)
                return BadRequest("User not found"); 

            // Compute hash of the provided password using the retrieved salt
            byte[] salt = Convert.FromBase64String(user.Credential.Salt);
            byte[] hash = GenerateHash(loginReq.Password, salt);

            // Compare the computed hash with the hash stored in the database and checks if password is correct
            if (!Convert.ToBase64String(hash).Equals(user.Credential.PasswordHash))
                return Unauthorized("Wrong password!");

            // When logged in saves UserId in session so it can be used in other endpoints
            HttpContext.Session.SetString("UserId", user.UserId.ToString());

            return Ok(new { message = $"Login successful! Welcome back {user.FirstName}!" });
        }

        // Generate the salt for password
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Generate the Hash with password and salt
        private byte[] GenerateHash(string password, byte[] salt)
        {

            byte[] hashedBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return hashedBytes;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Clear user's session
            HttpContext.Session.Clear();

            return Ok("Logout successful");
        }
    }

    // Models for login and register, move into model folder maybe
    public class RegisterRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
