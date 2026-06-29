using System.ComponentModel.DataAnnotations;

namespace VideoGameCharacter.Dtos
{
    //Used when the client wants to POST a new character. No Id because the database generates it automatically.
    //Although identical to UpdateCharacterRequest, both are kept separate as they represent different intentions and may diverge in the future.
    public class CreateCharacterRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Game { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;
    }
}