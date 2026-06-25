using Microsoft.AspNetCore.Mvc;
using VideoGameCharacter.Dtos;
using VideoGameCharacter.Services;

namespace VideoGameCharacter.Controllers
{
    [Route("api/[controller]")]     //[Route("api/[controller]")] defines the base URL for all endpoints in this controller.
    [ApiController]                 //[ApiController] is a attribute that enables several automatic behaviors, like automatic binding and problem detail responses.
    public class VideoGameCharacterController(IVideoGameCharacterService service, ILogger<VideoGameCharacterController> logger): ControllerBase      //ControllerBase because this project is a WebAPI and does not need View suppport.
    {
        [HttpGet]   //Maps to GET /api/VideoGameCharacter
        public async Task<ActionResult<List<CharacterResponse>>> GetCharacters()
        {
            try
            {
                var characters = await service.GetAllCharactersAsync();

                //No characters found in the database: log a warning and return 404.
                if(characters is null)
                {
                    logger.LogWarning($"{nameof(GetCharacters)}: There are no characters on the database.");
                    return NotFound("There are no characters on the database.");
                }

                //Characters retrieved successfully: log and return 200 with the list.
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

        [HttpPost]  //Maps to POST /api/VideoGameCharacter
        public async Task<ActionResult<CharacterResponse>> AddCharacter(CreateCharacterRequest character)
        {
            try
            {
                var createdCharacter = await service.AddCharacterAsync(character);

                //Character added successfuly: log and returns 200 with a Location header.
                logger.LogInformation($"{nameof(AddCharacter)}: New character added successfully on the database- {character.Name}, {character.Game}.");
                return CreatedAtAction(nameof(GetCharacter), new { id = createdCharacter.Id}, createdCharacter);
            }
            catch(InvalidOperationException e)
            {
                //Warning: log and return 409
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

        [HttpPut("{id}")]   //Maps to PUT /api/VideoGameCharacter/{id}
        public async Task<ActionResult> UpdateCharacter(int id, UpdateCharacterRequest character)
        {
            try
            {
                var updated = await service.UpdateCharacterAsync(id, character);

                //Character not found in the database: log a warning and return 404.
                if(!updated)
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
                //Warning: log and return 409
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

        [HttpDelete("{id}")]    //Maps to  DELETE /api/VideoGameCharacter/{id}
        public async Task<ActionResult> DeleteCharacter(int id)
        {
            try
            {
                var deleted = await service.DeleteCharacterAsync(id);

                //Character not found in the database: log a warning and return 404.
                if(!deleted)
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