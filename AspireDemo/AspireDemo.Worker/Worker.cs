using AspireDemo.Data.Contexts;
using AspireDemo.Data.Models;

namespace AspireDemo.Worker;

public class Worker(ILogger<Worker> logger, WeatherDbContext dbContext) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int fetchIntervalMinutes = 5;
        var envValue = Environment.GetEnvironmentVariable("FETCH_INTERVAL_MINUTES");
        if (int.TryParse(envValue, out var parsedMinutes) && parsedMinutes > 0)
        {
            fetchIntervalMinutes = parsedMinutes;
        }
        string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            dbContext.Weathers.AddRange(Enumerable.Range(1, 5).Select(index =>
                    new Weather
                    {
                        Id = Guid.NewGuid(),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Date = DateOnly.FromDateTime(DateTime.Today),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    }));
            dbContext.SaveChanges();

            await Task.Delay(TimeSpan.FromMinutes(fetchIntervalMinutes), stoppingToken);
        }
    }
}
