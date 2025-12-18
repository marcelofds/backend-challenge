using FinanceTest.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanceTest.Infrastructure.Contexts;

public class FinanceTestContext : DbContext
{
    public FinanceTestContext(DbContextOptions<FinanceTestContext> options) : base(options)
    {
    }

    public required DbSet<Transaction> Transactions { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceTestContext).Assembly);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        base.OnModelCreating(modelBuilder);
    }
}