using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VideoGameCharacter.Data;
using VideoGameCharacter.DTOs;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        //Adds a new user to the User's table with the provided DTO information, stores it on the table, with the hashed password and returns it as a UserResponse.
        public async Task<UserResponse> RegisterUserAsync(CreateUserRequest request)
        {
            //Checks if a user with the same Email already exists in the database.
            if(await context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower()))
                throw new InvalidOperationException($"The email '{request.Email}' already exists.");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                IsActive = request.IsActive,
                Role = request.Role.ToString()
            };

            //Hashes the plain-text password using ASP.NET Core Identity's built-in PasswordHasher (PBKDF2), so the raw password is never stored.
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role.ToString()
            };
        }
        
        //Authenticates a user by email and password. 
        //Verifies the hashed password using ASP.NET Core Identity's PasswordHasher, and rejects the login outright if the account is not active.
        public async Task<string> LoginAsync(LoginRequest request)
        {
            //Checks if a user with the provided Email exists in the database.
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            //If the user does not exist or the password is incorrect, an exception is thrown.
            if(user is null || (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed))
                throw new InvalidOperationException($"Email or password is incorrect.");

            //If the user is not active, an exception is thrown.
            if (!user.IsActive)
                throw new UnauthorizedAccessException($"User with email '{user.Email}' is inactive.");


            return CreateToken(user);
        }

        //Builds and signs a JWT for the given user, embedding their email and Id as claims.
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                //Used to identify the caller. DbClaimsTransformation looks up the user by this email on every request to re-derive their current Role and IsActive status.
                new Claim(ClaimTypes.Name, user.Email),

                //Carries the user's database Id, useful for actions that need to reference the caller directly (e.g. preventing a user from deleting their own account).
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                //No Role claim. It gets attached fresh on every request by DbClaimsTransformation.
            };

            //Loads the signing key from configuration and prepares the signing credentials used to prove this token was issued by this server and has not been tampered with.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Assembles the token: who issued it, who it's for, what claims it carries, when it expires, and how it is signed.
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            //Serializes the token into the final compact string format sent to the client.
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        //Returns all users information from the table.
        public async Task<List<UserResponse>> GetUsersAsync()
        {
            /*var users = await context.Users.ToListAsync();

            return users.Select(user => new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role.ToString()
            }).ToList();*/

            return await context.Users.Select(user => new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role.ToString()
            }).ToListAsync();
        }

        //Returns a user's information by Id from the table.
        public async Task<UserResponse?> GetUserByIdAsync(int id)
        {
            var result = await context.Users
                .Where(c => c.Id == id)
                .Select(c => new UserResponse
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    IsActive = c.IsActive,
                    Role = c.Role.ToString()
                }).FirstOrDefaultAsync();

            return result;
        }

        //Fully updates a user's information by Id in the table with the provided information.
        public async Task<bool> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            //Checks if there is a user with the specified Id in the table.
            var userToUpdate = await context.Users.FindAsync(id);

            if (userToUpdate is null)
                return false;

            //Checks if a user with the same email already exists in the table.
            if (await context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != id))
                throw new InvalidOperationException($"A user with the email '{request.Email}' already exists.");

            //Updates the user's information with the provided data.
            userToUpdate.FirstName = request.FirstName;
            userToUpdate.LastName = request.LastName;
            userToUpdate.Email = request.Email;
            userToUpdate.IsActive = request.IsActive;
            userToUpdate.Role = request.Role.ToString();

            //Password is re-hashed the same way as registration. The previous hash is fully replaced.
            userToUpdate.PasswordHash = new PasswordHasher<User>().HashPassword(userToUpdate, request.Password);

            //Saves the changes to the database. Generates and executes the UPDATE SQL.
            await context.SaveChangesAsync();

            return true;
        }

        //Finds a user by Id and updates only the fields included in the request, leaving anything omitted unchanged.
        public async Task<bool> PatchUserAsync(int id, PatchUserRequest request)
        {
            var userToUpdate = await context.Users.FindAsync(id);

            if (userToUpdate is null)
                return false;

            if (request.Email is not null && await context.Users.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != id))
                throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");

            if (request.FirstName is not null)
                userToUpdate.FirstName = request.FirstName;

            if (request.LastName is not null)
                userToUpdate.LastName = request.LastName;

            if (request.Email is not null)
                userToUpdate.Email = request.Email;

            if (request.IsActive is not null)
                userToUpdate.IsActive = request.IsActive.Value;

            //.Value unwraps the nullable UserRoles? into a plain UserRoles, since ToString() on the nullable type directly triggers a possible-null warning even though the null check above guarantees it's safe
            if (request.Role is not null)
                userToUpdate.Role = request.Role.Value.ToString(); 

            if (request.Password is not null)
                userToUpdate.PasswordHash = new PasswordHasher<User>().HashPassword(userToUpdate, request.Password);

            await context.SaveChangesAsync();

            return true;
        }

        //Deletes a user by Id from the table.
        public async Task<bool> DeleteUserAsync(int id, int currentUserId)
        {
            //Checks if there is a user with the specified Id in the table.
            var user = await context.Users.FindAsync(id);

            if (user is null)
                return false;

            if(id == currentUserId)
                throw new InvalidOperationException("You cannot delete your own user account.");

            //Removes the user from the database. Generates and executes the DELETE SQL.
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return true;
        }
    }
}