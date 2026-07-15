using Microsoft.EntityFrameworkCore;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.Data
{
    //AppDbContext is the class that coordinates Entity Framework Core and acts as the bridge between the C# code and the database.
    //DbContext is the configuration object for the context.
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        //DbSet is EF Core's representation of a database table in C#.
        //Represents the Characters table.
        public DbSet<Character> Characters => Set<Character>();

        //Represents the Users table. 
        //Holds the accounts used for JWT authentication and role-based authorization.
        public DbSet<User> Users => Set<User>();

        //Database constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Database constraint that does not allow the same email to be inserted twice, even at the same time.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            //Database constraint that does not allow the same Name+Game combination to be inserted twice, even at the same time.
            //Backs up the AnyAsync check already done in VideoGameCharacterService.AddCharacterAsync/UpdateCharacterAsync.
            modelBuilder.Entity<Character>()
                .HasIndex(c => new { c.Name, c.Game })
                .IsUnique();
        }
    }
}