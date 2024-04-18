namespace MatGPT.Models
{
    public class Credential
    {
        public int CredentialId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

    }
}
