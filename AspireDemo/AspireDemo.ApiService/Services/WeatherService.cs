using AspireDemo.Data.Contexts;
using AspireDemo.Data.Models;

namespace AspireDemo.ApiService.Services
{
    public class WeatherService
    {
        private WeatherDbContext _dbContext;

        public WeatherService(WeatherDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<Weather> GetTodaysWeather()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return [.. _dbContext.Weathers.Where(w => w.Date == today)];
        }

        public void AddTodaysWeather(int temperatureC, string? summary = null)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var weather = new Weather
            {
                TemperatureC = temperatureC,
                Date = today,
                Summary = summary
            };
            _dbContext.Weathers.Add(weather);
            _dbContext.SaveChanges();
        }
    }
}
