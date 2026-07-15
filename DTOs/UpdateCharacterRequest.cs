using System.ComponentModel.DataAnnotations;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.DTOs
{
    //Used when the client wants to PUT/update an existing character. No Id because it comes from the URL.
    //Although identical to CreateCharacterRequest, both are kept separate as they represent different intentions and may diverge in the future.
    public class UpdateCharacterRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Game { get; set; } = string.Empty;

        [Required]
        public CharacterRoles Role { get; set; } = CharacterRoles.Hero;
    }
}