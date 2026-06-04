using VehicleManagement.Infrastructure.Extensions;

LoadDotEnvFiles();

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddInfrastructure(config);

var app = builder.Build();
app.UseInfrastructure(app.Environment);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/health"),
    applicationBuilder => applicationBuilder.UseHttpsRedirection());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void LoadDotEnvFiles()
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var envFiles = new[]
    {
        Path.Combine(currentDirectory, ".env"),
        Path.Combine(currentDirectory, "VehicleManagement.Api", ".env")
    };

    foreach (var envFile in envFiles.Distinct().Where(File.Exists))
    {
        DotNetEnv.Env.NoClobber().Load(envFile);
    }
}

/// <summary>
/// Application entry point used by integration tests.
/// </summary>
public partial class Program;
