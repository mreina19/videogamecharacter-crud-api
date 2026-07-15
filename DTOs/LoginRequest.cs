using System.ComponentModel.DataAnnotations;

namespace VideoGameCharacter.DTOs
{
    //Used when the client POSTs to /api/auth/login. 
    //Public endpoint. Only Email and Password are needed to authenticate against an existing account.
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}