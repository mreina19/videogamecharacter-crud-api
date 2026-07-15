using VideoGameCharacter.DTOs;

namespace VideoGameCharacter.Services
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterUserAsync(CreateUserRequest request);
        Task<string> LoginAsync(LoginRequest request);
        Task<List<UserResponse>> GetUsersAsync();
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id, int currentUserId);
    }
}