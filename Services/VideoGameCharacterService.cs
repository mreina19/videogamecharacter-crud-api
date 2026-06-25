using Microsoft.EntityFrameworkCore;
using VideoGameCharacter.Data;
using VideoGameCharacter.Dtos;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.Services
{
    public class VideoGameCharacterService(AppDbContext context) : IVideoGameCharacterService
    {
        //Adds a new character to the database with the provided DTO information, stores it on the database and returns it as a CharacterResponse.
        public async Task<CharacterResponse> AddCharacterAsync(CreateCharacterRequest character)
        {
            //Checks if a character with the same Name and Game already exists in the database.
            //If so, the insert is rejected and an exception is thrown.
            if (await context.Characters.AnyAsync(c => c.Name.ToLower() == character.Name.ToLower() && c.Game.ToLower() == character.Game.ToLower()))
                throw new InvalidOperationException("A character with the same Name and Game already exists.");

            //Maps the incoming DTO to the Character model (entity)
            var newCharacter = new Character
            {
                Name = character.Name,
                Game = character.Game,
                Role = character.Role
            };

            //Add the new character to the database
            context.Characters.Add(newCharacter);

            //Saves the changes to the database. Generates and executes the INSERT SQL.
            await context.SaveChangesAsync();

            //Maps the saved entity back to a CharacterResponse object. The Id is now present in the database.
            return new CharacterResponse
            {
                Id = newCharacter.Id,
                Name = newCharacter.Name,
                Game = newCharacter.Game,
                Role = newCharacter.Role
            };
        }

        //Finds a character by Id and removes it from the database.
        public async Task<bool> DeleteCharacterAsync(int id)
        {
            //FindAsync looks up the entity by primary key.
            var characterToDelete = await context.Characters.FindAsync(id);

            if(characterToDelete is null)
                return false;

            //Marks the entity for deletion.
            context.Characters.Remove(characterToDelete);

            //Saves the changes. Generates and executes the DELETE SQL
            await context.SaveChangesAsync();

            return true;
        }

        //Returns all characters from the database as a list of CharacterResponse DTOs
        public async Task<List<CharacterResponse>> GetAllCharactersAsync() 
            => await context.Characters.Select(c=> new CharacterResponse
            {
                Id = c.Id,
                Name = c.Name,
                Game = c.Game,
                Role = c.Role,
            }).ToListAsync();

        //Returns a single character by Id as a CharacterResponse DTO, or null if not found.
        public async Task<CharacterResponse?> GetCharacterByIdAsync(int id)
        {
            var result = await context.Characters
                .Where(c => c.Id == id)
                .Select(c => new CharacterResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Game = c.Game,
                    Role = c.Role
                }).FirstOrDefaultAsync();

            return result;
        }

        //Finds a character by Id, updates its fields with the incoming DTO data, and saves the changes.
        public async Task<bool> UpdateCharacterAsync(int id, UpdateCharacterRequest character)
        {
            //Checks if a character with the same Name and Game (with a different Id) already exists in the database.
            //If so, the update is rejected and an exception is thrown.
            if (await context.Characters.AnyAsync(c => c.Name.ToLower() == character.Name.ToLower() && c.Game.ToLower() == character.Game.ToLower() && c.Id != id))
                throw new InvalidOperationException("A character with the same Name and Game already exists.");

            var characterToUpdate = await context.Characters.FindAsync(id);

            if(characterToUpdate is null)
                return false;

            //Updates each field. EF Core automatically tracks these changes. There is no DbContext update function.
            characterToUpdate.Name = character.Name;
            characterToUpdate.Game = character.Game;
            characterToUpdate.Role = character.Role;

            //Saves the changes. Generates and executes the UPDATE SQL.
            await context.SaveChangesAsync();

            return true;
        }
    }
}