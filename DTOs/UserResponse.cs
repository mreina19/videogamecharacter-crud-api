namespace VideoGameCharacter.DTOs
{
    //Returned to the client after registration, login, or a user query.
    //Excludes Password/PasswordHash, which should never leave the server.
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive {get; set;} = true;
        public string Role { get; set; } = string.Empty;
    }
}