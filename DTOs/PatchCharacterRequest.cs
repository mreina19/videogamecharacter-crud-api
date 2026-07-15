using System.ComponentModel.DataAnnotations;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.DTOs
{
    //Used when an admin partially updates a character's information via PATCH. 
    //No Id because it comes from the URL. Restricted to admins on the controller action.
    //All fields are optional. Only the ones actually included in the request are validated and applied.
    public class PatchCharacterRequest
    {
        [MinLength(1)]
        [MaxLength(50)]
        public string? Name { get; set; }

        [MinLength(1)]
        [MaxLength(50)]
        public string? Game { get; set; }

        public CharacterRoles? Role { get; set; }
    }
}