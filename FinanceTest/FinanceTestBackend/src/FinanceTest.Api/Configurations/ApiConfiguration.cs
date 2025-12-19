using System.Diagnostics.CodeAnalysis;
using FinanceTest.Api.Filters;
using FinanceTest.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

namespace FinanceTest.Api.Configurations;
[ExcludeFromCodeCoverage]
public static class ApiConfiguration
{
    public static void AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

        services.AddControllers(options => { options.Filters.Add<NotificationFilter>(); });

        services.AddOpenApi();

        var connectionString = configuration["DATABASE_URL"]
                               ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FinanceTestContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddCors(options =>
        {
            options.AddPolicy("General",
                builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
        });
    }

    public static void UseApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("FinanceTest API Docs")
                    .WithTheme(ScalarTheme.Mars)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        app.UseSerilogRequestLogging();

        app.UseCors("General");

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}