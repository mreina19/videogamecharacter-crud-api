using System.ComponentModel.DataAnnotations;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.DTOs
{
    //Used when the client wants to POST a new user (registration). 
    //No Id because the database generates it automatically. Password is plain-text from the client and gets hashed server-side before being stored.
    public class CreateUserRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        public string Password { get; set; } = string.Empty;    //General guidance is to cap between 64-128 characters.

        [Required]
        public bool IsActive {get; set;} = true;

        [Required]
        public UserRoles Role { get; set; }
    }
}