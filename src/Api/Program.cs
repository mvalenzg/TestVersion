using System.Reflection;

using Microsoft.AspNetCore.Http.HttpResults;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Test API")
        .WithTheme(ScalarTheme.Saturn)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/users", () =>
{
    var users = Enumerable.Range(1, 5).Select(index =>
        new
        {
            id = index,
            name = $"User {index}",
            email= $"user{index}@company.com"
        })
        .ToArray();
    return users;
})
.WithName("GetAllUSers")
.WithTags("Users");

app.MapGet("/users/id", (int id) =>
{
    return
        new
        {
            id = id,
            name = $"User {id}",
            email = $"user{id}@company.com"
        };
        
    
})
.WithName("GetUSerById")
.WithTags("Users");


app.MapGet("/version", () =>
{
    var assembly = Assembly.GetExecutingAssembly();
    var infoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
    .InformationalVersion;

    var fileVersion = assembly.GetName().Version?.ToString();

    return Results.Ok(new
    {
        Version = infoVersion ?? fileVersion,
        Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
        RunTime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
        ServerTime = DateTime.UtcNow
    });
}).WithName("Version")
.WithTags("Version");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
