using AspireDemo.Data.Contexts;
using Microsoft.EntityFrameworkCore;

internal class DbInitializer
{
    internal static void Initialize(WeatherDbContext dbContext, ILogger<DbInitializer> logger)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        if (dbContext.Database.IsRelational())
        {
            // Sjekk om man kjører en relasjonsdatabase (test og produksjon)
            // Hvis man kjører InMemoryDb så har den ikke migreringer
            logger.LogInformation("Database: Migrerer database endringer");
            dbContext.Database.Migrate();
            dbContext.SaveChanges();
        }
        else
        {
            var databaseExists = dbContext.Database.EnsureCreated();

            logger.LogInformation(databaseExists ? "Database eksisterte fra før av" : "Database eksisterte ikke fra før av, oppretter...");
        }
    }
}