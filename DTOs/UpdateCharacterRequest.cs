namespace VideoGameCharacter.Dtos
{
    //Used when the client wants to PUT/update an existing character. No Id because it comes from the URL.
    //Although identical to CreateCharacterRequest, both are kept separate as they represent different intentions and may diverge in the future.
    public class UpdateCharacterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Game { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}