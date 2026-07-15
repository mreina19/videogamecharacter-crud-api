using System.ComponentModel.DataAnnotations;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.DTOs
{
    //Used when the client wants to POST a new character. No Id because the database generates it automatically.
    //Although identical to UpdateCharacterRequest, both are kept separate as they represent different intentions and may diverge in the future.
    public class CreateCharacterRequest
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