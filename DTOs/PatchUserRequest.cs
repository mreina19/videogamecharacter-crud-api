using System.ComponentModel.DataAnnotations;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.DTOs
{
    //Used when an admin partially updates a user's account via PATCH. 
    //No Id because it comes from the URL. Restricted to admins on the controller action.
    //All fields are optional. Only the ones actually included in the request are validated and applied.
    public class PatchUserRequest
    {
        [MinLength(1)]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MinLength(1)]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MinLength(8)]
        [MaxLength(72)]
        public string? Password { get; set; }

        public bool? IsActive { get; set; }

        public UserRoles? Role { get; set; }
    }
}