using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameCharacter.DTOs;
using VideoGameCharacter.Services;

namespace VideoGameCharacter.Controllers
{
    [Route("api/[controller]")]     //[Route("api/[controller]")] defines the base URL for all endpoints in this controller.
    [ApiController]                 //[ApiController] is a attribute that enables several automatic behaviors, like automatic binding and problem detail responses.

    public class VideoGameCharacterController(IVideoGameCharacterService service, ILogger<VideoGameCharacterController> logger): ControllerBase      //ControllerBase because this project is a WebAPI and does not need View suppport.
    {
        [HttpGet]     //Maps to GET /api/VideoGameCharacter
        [Authorize]   //Requires authentication to access this endpoint.
        public async Task<ActionResult<List<CharacterResponse>>> GetCharacters()
        {
            try
            {
                var characters = await service.GetAllCharactersAsync();

                //Characters retrieved successfully: log and return 200 with the list (may be empty).
                logger.LogInformation($"{nameof(GetCharacters)}: All characters information retrieved successfully from the database.");
                return Ok(characters);
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500.
                logger.LogError($"{nameof(GetCharacters)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id}")]   //Maps to GET /api/VideoGameCharacter/{id}
        [Authorize]         //Requires authentication to access this endpoint.
        public async Task<ActionResult<CharacterResponse>> GetCharacter(int id)
        {
            try
            {            
                var character = await service.GetCharacterByIdAsync(id);

                //Character not found in the database: log a warning and return 404.
                if(character is null)
                {
                    logger.LogWarning($"{nameof(GetCharacter)}: There is no character '{id}' on the database.");
                    return NotFound($"There is no character '{id}' on the database.");
                }

                //Character retrieved successfully: log and return 200 with the character.
                logger.LogInformation($"{nameof(GetCharacter)}: Character '{id}' information retrieved successfully from the database.");
                return Ok(character);
            }
            catch(Exception e)
            {
                //Unexpected error: log and return 500.
                logger.LogError($"{nameof(GetCharacter)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]                         //Maps to POST /api/VideoGameCharacter
        [Authorize(Roles = "Admin")]       //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<ActionResult<CharacterResponse>> AddCharacter(CreateCharacterRequest character)
        {
            try
            {
                var createdCharacter = await service.AddCharacterAsync(character);

                //Character added successfuly: log and returns 201 with a Location header.
                logger.LogInformation($"{nameof(AddCharacter)}: New character added successfully on the database- {character.Name}, {character.Game}.");
                return CreatedAtAction(nameof(GetCharacter), new { id = createdCharacter.Id}, createdCharacter);
            }
            catch(InvalidOperationException e)
            {
                //Duplicate character: log and return 409
                logger.LogWarning($"{nameof(AddCharacter)}: {e.Message}");
                return Conflict(e.Message);
            }
            catch(Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(AddCharacter)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{id}")]                  //Maps to PUT /api/VideoGameCharacter/{id}
        [Authorize(Roles = "Admin")]       //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<ActionResult> UpdateCharacter(int id, UpdateCharacterRequest character)
        {
            try
            {
                //Character not found: log a warning and return 404.
                if(!await service.UpdateCharacterAsync(id, character))
                {
                    logger.LogWarning($"{nameof(UpdateCharacter)}: There is no character '{id}' on the database.");
                    return NotFound($"There is no character '{id}' on the database.");
                }

                //Character updated successfully: log and return 204.
                logger.LogInformation($"{nameof(UpdateCharacter)}: Character '{id}' information updated successfully in the database.");
                return NoContent();
            }
            catch(InvalidOperationException e)
            {
                //Duplicate character: log and return 409
                logger.LogWarning($"{nameof(UpdateCharacter)}: {e.Message}");
                return Conflict(e.Message);
            }
            catch(Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(UpdateCharacter)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch("{id}")]             //Maps to PATCH /api/VideoGameCharacter/{id}
        [Authorize(Roles = "Admin")]    //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<ActionResult> PatchCharacter(int id, PatchCharacterRequest character)
        {
            try
            {
                //Character not found: log a warning and return 404.
                if (!await service.PatchCharacterAsync(id, character))
                {
                    logger.LogWarning($"{nameof(PatchCharacter)}: There is no character '{id}' on the database.");
                    return NotFound($"There is no character '{id}' on the database.");
                }

                //Character updated successfully: log and return 204.
                logger.LogInformation($"{nameof(PatchCharacter)}: Character '{id}' partially updated successfully in the database.");
                return NoContent();
            }
            catch (InvalidOperationException e)
            {
                //Duplicate character: log and return 409
                logger.LogWarning($"{nameof(PatchCharacter)}: {e.Message}");
                return Conflict(e.Message);
            }
            catch (Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(PatchCharacter)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{id}")]               //Maps to  DELETE /api/VideoGameCharacter/{id}
        [Authorize(Roles = "Admin")]       //Restricts access to this endpoint to users with the "Admin" role.
        public async Task<ActionResult> DeleteCharacter(int id)
        {
            try
            {
                //Character not found in the database: log a warning and return 404.
                if(!await service.DeleteCharacterAsync(id))
                {
                    logger.LogWarning($"{nameof(DeleteCharacter)}: There is no character '{id}' on the database.");
                    return NotFound($"There is no character '{id}' on the database.");
                }

                //Character deleted successfully: log and return 204.
                logger.LogInformation($"{nameof(DeleteCharacter)}: Character '{id}' information deleted successfully from the database.");
                return NoContent();
            }
            catch(Exception e)
            {
                //Unexpected error: log and return 500
                logger.LogError($"{nameof(DeleteCharacter)}: {e.Message}.");
                return StatusCode(500, e.Message);
            }
        }
    }
}