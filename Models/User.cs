//Represents the Users table in the database.
//Each property maps to a column in the table. The class is decorated with data annotations to specify validation rules and constraints for each property.
//Users authenticate via email/password and receive a JWT, required to insert, update, or delete records in the Characters table.

using System.ComponentModel.DataAnnotations;

namespace VideoGameCharacter.Models
{    
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(70)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash{ get; set; } = string.Empty;

        [Required]
        public bool IsActive {get; set;} = true;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = string.Empty;
    }
}