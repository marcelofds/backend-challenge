using FinanceTest.Domain.Entities.Transactions;
using FinanceTest.Domain.Enums;
using FluentAssertions;

namespace FinanceTest.Tests.Domain;

public class TransactionTests
{
    [Theory]
    [InlineData(TransactionType.Debit, 100.00, 100.00)]                // Type 1: Inflow (+)
    [InlineData(TransactionType.Boleto, 100.00, -100.00)]               // Type 2: Outflow (-)
    [InlineData(TransactionType.Financing, 500.00, -500.00)]        // Type 3: Outflow (-)
    [InlineData(TransactionType.Credit, 100.00, 100.00)]               // Type 4: Inflow (+)
    [InlineData(TransactionType.LoanReceipt, 1000.00, 1000.00)] // Type 5: Inflow (+)
    [InlineData(TransactionType.Sales, 200.00, 200.00)]                // Type 6: Inflow (+)
    [InlineData(TransactionType.TEDReceipt, 150.00, 150.00)]        // Type 7: Inflow (+)
    [InlineData(TransactionType.DOCReceipt, 150.00, 150.00)]        // Type 8: Inflow (+)
    [InlineData(TransactionType.Rent, 800.00, -800.00)]              // Type 9: Outflow (-)
    public void GetSignedValue_ShouldReturnCorrectSign_BasedOnTransactionType(
        TransactionType type, 
        decimal originalValue, 
        decimal expectedSignedValue)
    {
        // Arrange
        var transaction = new Transaction(
            (int)type, 
            DateTime.UtcNow, 
            originalValue, 
            "01234567890", 
            "1234****5678", 
            "John Doe", 
            "My Store"
        );

        // Act
        var result = transaction.GetSignedValue();

        // Assert
        result.Should().Be(expectedSignedValue, 
            because: $"transaction type {type} should have specific sign logic");
    }

    [Fact]
    public void Validate_ShouldReturnValidResult_WhenTransactionIsValid()
    {
        // Arrange
        var transaction = new Transaction(
            1,
            DateTime.UtcNow,
            100,
            "01234567890",
            "1234****5678",
            "John Doe",
            "My Store"
        );

        // Act
        var result = transaction.Validate();

        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public void Validate_ShouldReturnInvalidResult_WhenTypeIsInvalid()
    {
        // Arrange
        var transaction = new Transaction(
            -1, 
            DateTime.UtcNow, 
            100,
            "01234567890",
            "1234****5678", 
            "Owner", 
            "Store");

        // Act
        var result = transaction.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Invalid transaction type."));
    }
    
    [Fact]
    public void Validate_ShouldReturnInvalidResult_WhenValueIsInvalid()
    {
        // Arrange
        var transaction = new Transaction(
            1, 
            DateTime.UtcNow, 
            0,
            "01234567890",
            "1234****5678", 
            "Owner", 
            "Store");

        // Act
        var result = transaction.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Transaction value must be greater than zero."));
    }

    [Fact]
    public void Validate_ShouldReturnInvalidResult_WhenCpfIsInvalid()
    {
        // Arrange
        var transaction = new Transaction(
            1, 
            DateTime.UtcNow, 
            100,
            string.Empty,
            "1234****5678", 
            "Owner", 
            "Store");

        // Act
        var result = transaction.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("CPF is required."));
    }
    
    [Fact]
    public void Validate_ShouldReturnInvalidResult_WhenCardNumberInvalid()
    {
        // Arrange
        var transaction = new Transaction(
            1, 
            DateTime.UtcNow, 
            100,
            "01234567890", 
            "", 
            "Owner",
            string.Empty);

        // Act
        var result = transaction.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Card number is required."));
    }
    
    [Fact]
    public void Validate_ShouldReturnInvalidResult_WhenStoreOwnerIsInvalid()
    {
        // Arrange
        var transaction = new Transaction(
            1, 
            DateTime.UtcNow, 
            100,
            "01234567890", 
            "1234****5678", 
            "",
            string.Empty);

        // Act
        var result = transaction.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Store owner name is required."));
    }

    [Fact]
    public void Validate_ShouldReturnInvalidResult_WhenStoreNameIsInvalid()
    {
        // Arrange
        var transaction = new Transaction(
            1, 
            DateTime.UtcNow, 
            100,
            "01234567890", 
            "1234****5678", 
            "Owner",
            string.Empty);

        // Act
        var result = transaction.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Store name is required."));
    }

    [Fact]
    public void Constructor_ShouldTrimStringFields()
    {
        // Arrange
        const string owner = "  John Doe  ";
        const string store = "  Super Store  ";

        // Act
        var transaction = new Transaction(
            1, 
            DateTime.Now, 
            100,
            "01234567890", 
            "1234****5678", 
            owner, 
            store);

        // Assert
        transaction.StoreOwner.Should().Be("John Doe");
        transaction.StoreName.Should().Be("Super Store");
    }
}