using FinanceTest.Domain.Entities.Transactions;
using FinanceTest.Infrastructure.Contexts;
using FinanceTest.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceTest.Tests.Repositories;

public class TransactionRepositoryTests
{
    private FinanceTestContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<FinanceTestContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        // Use Activator to bypass required property initialization
        var context = (FinanceTestContext)Activator.CreateInstance(typeof(FinanceTestContext), options)!;

        // Ensure the database is created
        context.Database.EnsureCreated();

        return context;
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddTransactionsToDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TransactionRepository(context);

        var transactions = new List<Transaction>
        {
            new Transaction(1, DateTime.UtcNow, 100, "12345678901", "1234****5678", "Owner 1", "Store 1"),
            new Transaction(2, DateTime.UtcNow, 200, "98765432109", "9876****4321", "Owner 2", "Store 2")
        };

        // Act
        await repository.AddRangeAsync(transactions);

        // Assert
        var result = await context.Transactions.ToListAsync();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTransactions_OrderedByStoreAndDate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TransactionRepository(context);

        var date1 = DateTime.UtcNow.AddDays(-2);
        var date2 = DateTime.UtcNow.AddDays(-1);
        var date3 = DateTime.UtcNow;

        var transactions = new List<Transaction>
        {
            new Transaction(1, date2, 100, "12345678901", "1234****5678", "Owner", "Store B"),
            new Transaction(2, date1, 200, "98765432109", "9876****4321", "Owner", "Store A"),
            new Transaction(3, date3, 150, "11111111111", "1111****1111", "Owner", "Store B")
        };

        await repository.AddRangeAsync(transactions);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].StoreName.Should().Be("Store A");
        result[1].StoreName.Should().Be("Store B");
        result[2].StoreName.Should().Be("Store B");
        result[1].Date.Should().BeBefore(result[2].Date);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoTransactions()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TransactionRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByStoreAsync_ShouldReturnTransactionsForSpecificStore()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TransactionRepository(context);

        var transactions = new List<Transaction>
        {
            new Transaction(1, DateTime.UtcNow, 100, "12345678901", "1234****5678", "Owner", "Store A"),
            new Transaction(2, DateTime.UtcNow, 200, "98765432109", "9876****4321", "Owner", "Store B"),
            new Transaction(3, DateTime.UtcNow, 150, "11111111111", "1111****1111", "Owner", "Store A")
        };

        await repository.AddRangeAsync(transactions);

        // Act
        var result = (await repository.GetByStoreAsync("Store A")).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.StoreName == "Store A");
    }

    [Fact]
    public async Task GetByStoreAsync_ShouldReturnEmpty_WhenStoreHasNoTransactions()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TransactionRepository(context);

        var transactions = new List<Transaction>
        {
            new Transaction(1, DateTime.UtcNow, 100, "12345678901", "1234****5678", "Owner", "Store A")
        };

        await repository.AddRangeAsync(transactions);

        // Act
        var result = await repository.GetByStoreAsync("Store B");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByStoreAsync_ShouldReturnEmpty_WhenDatabaseIsEmpty()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new TransactionRepository(context);

        // Act
        var result = await repository.GetByStoreAsync("Store A");

        // Assert
        result.Should().BeEmpty();
    }
}
