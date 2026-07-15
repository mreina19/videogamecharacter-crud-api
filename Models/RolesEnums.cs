//Using an enum instead of a plain string rejects invalid roles automatically at model-binding time, and shows a dropdown of valid options in Swagger.

namespace VideoGameCharacter.Models
{
    //Fixed set of valid roles a user can hold.
    public enum UserRoles { Admin, Normal }

    //Fixed set of valid narrative roles a character can hold.
    public enum CharacterRoles { Hero, Villain, Figurant, AntiHero }
}