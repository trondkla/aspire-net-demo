using AspireDemo.Data.Contexts;
using AspireDemo.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<WeatherDbContext>((options) =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<Worker>();
builder.Services.AddHostedService<ScopedWorkerService>();

var host = builder.Build();
host.Run();

public class ScopedWorkerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ScopedWorkerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var worker = scope.ServiceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(stoppingToken);
    }
}
