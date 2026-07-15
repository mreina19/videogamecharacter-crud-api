using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameCharacter.DTOs;
using VideoGameCharacter.Services;

namespace VideoGameCharacter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService service, ILogger<AuthController> logger): ControllerBase
    {
        [HttpGet("users")]                 //Maps to GET /api/Auth/users
        [Authorize(Roles = "Admin")]       //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            try
            {
                var users = await service.GetUsersAsync();

                //Users retrieved successfully: log and return 200 with the list (may be empty).
                logger.LogInformation($"{nameof(GetUsers)}: All users information retrieved successfully from the database.");
                return Ok(users);
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(GetUsers)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("users/{id}")]            //Maps to GET /api/Auth/users/{id}
        [Authorize(Roles = "Admin")]       //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<ActionResult<UserResponse>> GetUser(int id)
        {
            try
            {
                var user = await service.GetUserByIdAsync(id);

                //User not found in the database: log a warning and return 404.
                if(user is null)
                {
                    logger.LogWarning($"{nameof(GetUser)}: User '{id}' not found.");
                    return NotFound($"User '{id}' not found.");
                }

                //User retrieved successfully: log and return 200 with the user.
                logger.LogInformation($"{nameof(GetUser)}: User '{id}' information retrieved successfully from the database.");
                return Ok(user);
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(GetUser)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("register")]              //Maps to POST api/Auth/register
        [Authorize(Roles = "Admin")]        //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<IActionResult> RegisterUser(CreateUserRequest request)
        {
            try
            {
                var userResponse = await service.RegisterUserAsync(request);

                //User added successfully: log and return 201 with a Location header.
                logger.LogInformation($"{nameof(RegisterUser)}: User '{userResponse.Id}' registered successfully.");
                return CreatedAtAction(nameof(GetUser), new { id = userResponse.Id }, userResponse);
            }
            catch (InvalidOperationException e)
            {
                //Email already exists: log and return 409
                logger.LogWarning($"{nameof(RegisterUser)}: {e.Message}");
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(RegisterUser)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("users/{id}")]        //Maps to PUT /api/Auth/users/{id}
        [Authorize(Roles = "Admin")]   //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            try
            {
                //User not found: log a warning and return 404.
                if(!await service.UpdateUserAsync(id, request))
                {
                    logger.LogWarning($"{nameof(UpdateUser)}: User '{id}' not found.");
                    return NotFound($"User '{id}' not found.");
                }

                //User updated successfully: log and return 204 No Content.
                logger.LogInformation($"{nameof(UpdateUser)}: User '{id}' updated successfully.");
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                //Email already exists: log and return 409
                logger.LogWarning($"{nameof(UpdateUser)}: {e.Message}");
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(UpdateUser)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch("users/{id}")]       //Maps to PATCH /api/Auth/users/{id}
        [Authorize(Roles = "Admin")]    //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<IActionResult> PatchUser(int id, PatchUserRequest request)
        {
            try
            {
                //User not found: log a warning and return 404.
                if (!await service.PatchUserAsync(id, request))
                {
                    logger.LogWarning($"{nameof(PatchUser)}: User '{id}' not found.");
                    return NotFound($"User '{id}' not found.");
                }

                //User updated successfully: log and return 204 No Content.
                logger.LogInformation($"{nameof(PatchUser)}: User '{id}' partially updated successfully.");
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                //Email already exists: log and return 409
                logger.LogWarning($"{nameof(PatchUser)}: {e.Message}");
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(PatchUser)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("users/{id}")]          //Maps to DELETE /api/Auth/users/{id}
        [Authorize(Roles = "Admin")]        //Restricts access to users with the "Admin" role.
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                //Extracts the user Id from the JWT token claims.
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                //User Id claim not found in the token: log a warning and return 401 Unauthorized.
                if (userIdClaim is null)
                {
                    logger.LogWarning($"{nameof(DeleteUser)}: Invalid Token. User Id claim not found.");
                    return Unauthorized("Invalid token. User Id claim not found.");
                }

                int currentUserId = int.Parse(userIdClaim.Value);

                //User not found: log a warning and return 404 Not Found.
                if(!await service.DeleteUserAsync(id, currentUserId))
                {
                    logger.LogWarning($"{nameof(DeleteUser)}: User '{id}' not found.");
                    return NotFound($"User '{id}' not found.");
                }

                //User deleted successfully: log and return 204 No Content.
                logger.LogInformation($"{nameof(DeleteUser)}: User '{id}' deleted successfully.");
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                //Attempt self delete: log and return 403 
                logger.LogWarning($"{nameof(DeleteUser)}: {e.Message}");
                return StatusCode(403, new { message = e.Message });
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(DeleteUser)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("login")]     //Maps to POST api/Auth/login
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var token = await service.LoginAsync(request);

                //Login successful: log and return 200 with the JWT token.
                logger.LogInformation($"{nameof(Login)}: User with email '{request.Email}' logged in successfully.");
                return Ok(new { Token = token });
            }
            catch (InvalidOperationException e)
            {
                //User does not exist, or incorrect credentials: log and return 401
                logger.LogWarning($"{nameof(Login)}: {e.Message}");
                return Unauthorized(e.Message);
            }
            catch(UnauthorizedAccessException e)
            {
                //Correct credentials, but the account is inactive: log and return 403. 
                logger.LogWarning($"{nameof(Login)}: {e.Message}");
                return StatusCode(403, new { message = e.Message });
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(Login)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("verify-token")]     //Maps to GET api/Auth/verify-token
        [Authorize]                   //Requires a valid JWT token to access this endpoint.
        public async Task<IActionResult> VerifyToken()
        {
           try
            {
                //Extracts the user Id from the JWT token claims.
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                //User Id claim not found in the token: log a warning and return 401 Unauthorized.
                if (userIdClaim is null)
                {
                    logger.LogWarning($"{nameof(VerifyToken)}: Invalid token. User Id claim not found.");
                    return Unauthorized("Invalid token. User Id claim not found.");
                }

                int userId = int.Parse(userIdClaim.Value);

                //Retrieves the user information.
                var userResponse = await service.GetUserByIdAsync(userId);

                //User does not exist or is inactive: log a warning and return 403 Forbidden.
                if (userResponse is null || !userResponse.IsActive)
                {
                    logger.LogWarning($"{nameof(VerifyToken)}: User '{userId}' no longer exists or is inactive.");
                    return StatusCode(403, new { message = $"User '{userId}' no longer exists or is inactive." });
                }

                //Valid token: Log and return 200 with the user information.
                logger.LogInformation($"{nameof(VerifyToken)}: Token verified successfully for user '{userId}'.");
                return Ok(userResponse);
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(VerifyToken)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }
    }
}