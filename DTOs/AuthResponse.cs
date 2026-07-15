namespace VideoGameCharacter.DTOs
{
    //Returned to the client after a successful login. Carries the signed JWT.
    //Required to access endpoints protected by [Authorize].
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
    }
}