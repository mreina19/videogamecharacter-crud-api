using Microsoft.AspNetCore.Mvc;
using VideoGameCharacter.Dtos;
using VideoGameCharacter.Services;

namespace VideoGameCharacter.Controllers
{
    [Route("api/[controller]")]     //[Route("api/[controller]")] defines the base URL for all endpoints in this controller.
    [ApiController]                 //[ApiController] is a convenience attribute that enables several automatic behaviors, like automatic binding and problem detail responses.
    public class VideoGameCharacterController(IVideoGameCharacterService service): ControllerBase      //ControllerBase because this project is a WebAPI and does not need View suppport.
    {
        /* Old Constructor way (pre C# 12)
            private readonly IVideoGameCharacterService _service;
            public VideoGameCharacterController(IVideoGameCharacterService service)
            {
                _service = service;
            }
        */

        [HttpGet]   //Maps to GET /api/VideoGameCharacter
        public async Task<ActionResult<List<CharacterResponse>>> GetCharacters()
        {
            var characters = await service.GetAllCharactersAsync();

            //If the list is null, return 404. Otherwise return 200 with the list of characters.
            return characters is null ? NotFound("There are no characters on the database.") : Ok(characters);
        }

        [HttpGet("{id}")]   //Maps to GET /api/VideoGameCharacter/{id}
        public async Task<ActionResult<CharacterResponse>> GetCharacter(int id)
        {
            var character = await service.GetCharacterByIdAsync(id);

            //If no character was found with that Id, return 404.
            if(character is null)
                return NotFound($"Character '{id}' was not found.");

            //Return 200 with the character.
            return Ok(character);

            //return character is null ? NotFound($"Character '{id}' was not found.") : Ok(character);
        }

        [HttpPost]  //Maps to POST /api/VideoGameCharacter
        public async Task<ActionResult<CharacterResponse>> AddCharacter(CreateCharacterRequest character)
        {
            var createdCharacter = await service.AddCharacterAsync(character);

            //Returns 201 Created with a Location header pointing to GET /api/VideoGameCharacter/{id}.
            return CreatedAtAction(nameof(GetCharacter), new { id = createdCharacter.Id}, createdCharacter);
        }

        [HttpPut("{id}")]   //Maps to PUT /api/VideoGameCharacter/{id}
        public async Task<ActionResult> UpdateCharacter(int id, UpdateCharacterRequest character)
        {
            var updated = await service.UpdateCharacterAsync(id, character);

            //If the service returns false, the character was not found. Return 404.
            //If the service returns true, the character was updated. Return 204 No Content.
            return updated ? NoContent() : NotFound($"Character '{id}' was not found.");
        }

        [HttpDelete("{id}")]    //Maps to  DELETE /api/VideoGameCharacter/{id}
        public async Task<ActionResult> DeleteCharacter(int id)
        {
            var deleted = await service.DeleteCharacterAsync(id);

            //If the service returns false, the character was not found. Return 404.
            //If the service returns true, the character was deleted. Return 204 No Content.
            return deleted ? NoContent() : NotFound($"Character '{id}' was not found.");
        }
    }
}