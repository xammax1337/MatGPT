using System.ComponentModel.DataAnnotations;

namespace MatGPT.Models
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
