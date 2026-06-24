using Microsoft.EntityFrameworkCore;
using VideoGameCharacter.Models;

namespace VideoGameCharacter.Data
{
    //AppDbContext is the class that coordinates Entity Framework Core and acts as the bridge between the C# code and the database.
    //DbContext is the configuration object for the context.
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        /*  Old Constructor way (pre C# 12)
            public AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
            {
                *Nothing here. Options are passed directly to DbContext base class.*
            }
        */

        //DbSet is EF Core's representation of a database table in C#.
        public DbSet<Character> Characters => Set<Character>();
    }
}