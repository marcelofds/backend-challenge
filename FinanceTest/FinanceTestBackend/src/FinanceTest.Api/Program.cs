using System.Diagnostics.CodeAnalysis;
using FinanceTest.Api.Configurations;
using Serilog;

namespace FinanceTest.Api;
[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        var builder = WebApplication.CreateBuilder(args);

        // Config explicitly the port
        var port = Environment.GetEnvironmentVariable("PORT") ?? "5289";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

        builder.Host.UseSerilog();

        builder.Services.AddApi(builder.Configuration);
        builder.Services.AddDependencyInjection();

        var app = builder.Build();

        app.ApplyMigrations();

        app.UseApi();

        app.Run();
    }
}