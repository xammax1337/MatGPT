using MatGPT.Data;
using MatGPT.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MatGPT.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private IConfiguration _config;
        
        public AuthController(ApplicationContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerReq)
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
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginReq)
        {
            
            // Finds the user trying to login using Email as username
            var user = await _context.Users
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u =>
                    u.Credential.Email == loginReq.Email);

            // Checks if the user exists
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // Compute hash of the provided password using the retrieved salt
            byte[] salt = Convert.FromBase64String(user.Credential.Salt);
            byte[] hash = GenerateHash(loginReq.Password, salt);

            // Compare the computed hash with the hash stored in the database and checks if password is correct
            if (!Convert.ToBase64String(hash).Equals(user.Credential.PasswordHash))
                return Unauthorized("Wrong password!");

            // Generate JWT token
            var token = GenerateJwt(user);

            return Ok(new { message = $"Login successful! Welcome back {user.FirstName} {user.LastName}!", token });
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
        public async Task<IActionResult> LogoutAsync()
        {
            // Clear user's session
            //HttpContext.Session.Clear();

            // Front end needs to add a "localStorage.removeItem('jwtToken') or sessionStorage.removeItem('jwtToken').
            // Since the JWT is stored there, so backend doesnt need to clean it, we just send a confirmation when user presses button.
            return Ok("Logout successful");
        }

        private string GenerateJwt(User user)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT_KEY not found in environment variables.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
