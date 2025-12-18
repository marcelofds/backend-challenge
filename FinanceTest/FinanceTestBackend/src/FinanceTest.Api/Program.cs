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

        builder.Host.UseSerilog();
        
        builder.Services.AddApi(builder.Configuration);
        builder.Services.AddDependencyInjection();

        var app = builder.Build();
        
        app.ApplyMigrations();
        
        app.UseApi();
        
        app.Run();
    }
}