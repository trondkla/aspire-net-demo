using AspireDemo.ApiService.Services;
using AspireDemo.Data.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WeatherDbContext>((options) =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddScoped<DbInitializer>();
builder.Services.AddScoped<WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/weatherforecast", (WeatherService weatherService) =>
{
    return weatherService.GetTodaysWeather();
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

app.Run();
