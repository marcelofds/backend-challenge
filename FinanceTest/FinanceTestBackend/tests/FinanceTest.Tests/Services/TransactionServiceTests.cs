using FinanceTest.Application.Dtos;
using FinanceTest.Application.Interfaces;
using FinanceTest.Application.Services;
using FinanceTest.Domain.Entities.Transactions;
using FinanceTest.Domain.Entities.Transactions.Repositories;
using FinanceTest.Domain.Enums;
using FluentAssertions;
using Moq;

namespace FinanceTest.Tests.Services;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _mockRepository;
    private readonly Mock<IFileParser> _mockParser;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _mockRepository = new Mock<ITransactionRepository>();
        _mockParser = new Mock<IFileParser>();
        _service = new TransactionService(_mockRepository.Object, _mockParser.Object);
    }

    [Fact]
    public async Task ProcessFileAsync_ShouldParseAndSaveTransactions_WhenStreamIsValid()
    {
        // Arrange
        var stream = new MemoryStream();
        var transactions = new List<Transaction>
        {
            new Transaction(1, DateTime.UtcNow, 100, "12345678901", "1234****5678", "Owner 1", "Store 1"),
            new Transaction(2, DateTime.UtcNow, 200, "98765432109", "9876****4321", "Owner 2", "Store 2")
        };

        _mockParser.Setup(p => p.Parse(stream)).Returns(transactions);
        _mockRepository.Setup(r => r.AddRangeAsync(It.IsAny<List<Transaction>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessFileAsync(stream);

        // Assert
        _mockParser.Verify(p => p.Parse(stream), Times.Once);
        _mockRepository.Verify(r => r.AddRangeAsync(It.Is<List<Transaction>>(list => list.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task ProcessFileAsync_ShouldNotSaveTransactions_WhenParserReturnsEmpty()
    {
        // Arrange
        var stream = new MemoryStream();
        var transactions = new List<Transaction>();

        _mockParser.Setup(p => p.Parse(stream)).Returns(transactions);

        // Act
        await _service.ProcessFileAsync(stream);

        // Assert
        _mockParser.Verify(p => p.Parse(stream), Times.Once);
        _mockRepository.Verify(r => r.AddRangeAsync(It.IsAny<List<Transaction>>()), Times.Never);
    }

    [Fact]
    public async Task GetStoreBalancesAsync_ShouldReturnBalancesGroupedByStore()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new Transaction((int)TransactionType.Debit, DateTime.UtcNow, 100, "12345678901", "1234****5678", "Owner", "Store A"),
            new Transaction((int)TransactionType.Boleto, DateTime.UtcNow, 50, "12345678901", "1234****5678", "Owner", "Store A"),
            new Transaction((int)TransactionType.Sales, DateTime.UtcNow, 200, "98765432109", "9876****4321", "Owner", "Store B")
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetStoreBalancesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var storeA = result.First(s => s.StoreName == "Store A");
        storeA.Operations.Should().HaveCount(2);
        storeA.TotalBalance.Should().Be(50); // 100 - 50

        var storeB = result.First(s => s.StoreName == "Store B");
        storeB.Operations.Should().HaveCount(1);
        storeB.TotalBalance.Should().Be(200);
    }

    [Fact]
    public async Task GetStoreBalancesAsync_ShouldReturnEmptyList_WhenNoTransactionsExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Transaction>());

        // Act
        var result = await _service.GetStoreBalancesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStoreBalancesAsync_ShouldCalculateCorrectBalances_WithMultipleTransactionTypes()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new Transaction((int)TransactionType.Debit, DateTime.UtcNow, 100, "12345678901", "1234****5678", "Owner", "Store X"),
            new Transaction((int)TransactionType.Credit, DateTime.UtcNow, 200, "12345678901", "1234****5678", "Owner", "Store X"),
            new Transaction((int)TransactionType.Boleto, DateTime.UtcNow, 50, "12345678901", "1234****5678", "Owner", "Store X"),
            new Transaction((int)TransactionType.Financing, DateTime.UtcNow, 30, "12345678901", "1234****5678", "Owner", "Store X")
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetStoreBalancesAsync();

        // Assert
        result.Should().HaveCount(1);
        var store = result.First();
        store.StoreName.Should().Be("Store X");
        store.TotalBalance.Should().Be(220); // 100 + 200 - 50 - 30
        store.Operations.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetStoreBalancesAsync_ShouldIncludeAllOperationDetails()
    {
        // Arrange
        var testDate = DateTime.UtcNow;
        var transactions = new List<Transaction>
        {
            new Transaction((int)TransactionType.Debit, testDate, 123.45m, "12345678901", "1234****5678", "Owner", "Store Y")
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _service.GetStoreBalancesAsync();

        // Assert
        var operation = result.First().Operations.First();
        operation.Type.Should().Be(TransactionType.Debit);
        operation.Date.Should().Be(testDate);
        operation.Value.Should().Be(123.45m);
        operation.Cpf.Should().Be("12345678901");
        operation.Card.Should().Be("1234****5678");
        operation.SignedValue.Should().Be(123.45m);
    }
}
