using System.ComponentModel.DataAnnotations;

namespace VideoGameCharacter.Dtos
{
    //Used when the client wants to PUT/update an existing character. No Id because it comes from the URL.
    //Although identical to CreateCharacterRequest, both are kept separate as they represent different intentions and may diverge in the future.
    public class UpdateCharacterRequest
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