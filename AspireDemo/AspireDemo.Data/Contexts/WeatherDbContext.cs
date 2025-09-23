using AspireDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
namespace AspireDemo.Data.Contexts
{
    public class WeatherDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Weather> Weathers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Weather>();
        }
    }
}
