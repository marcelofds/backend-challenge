using System.Diagnostics.CodeAnalysis;
using FinanceTest.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceTest.Api.Configurations;
[ExcludeFromCodeCoverage]
public static class MigrationConfiguration
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<FinanceTestContext>();
                
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações do banco de dados.");
        }
    }
}