using System.ComponentModel.DataAnnotations;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.DTOs
{
    //Used when an admin updates another user's account, including resetting their password and reassigning their role. 
    //No Id because it comes from the URL. Restricted to admins on the controller action.
    public class UpdateUserRequest
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