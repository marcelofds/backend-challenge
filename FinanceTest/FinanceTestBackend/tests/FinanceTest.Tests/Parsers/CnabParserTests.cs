using System.Text;
using FinanceTest.Domain.Enums;
using FinanceTest.Domain.Notifications.Events;
using FinanceTest.Domain.Notifications.Handlers;
using FinanceTest.Infrastructure.Parsers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTest.Tests.Parsers;

public class CnabParserTests
{
    private readonly Mock<ILogger<CnabParser>> _mockLogger;
    private readonly Mock<IMediatorHandler> _mockBus;
    private readonly CnabParser _parser;

    public CnabParserTests()
    {
        _mockLogger = new Mock<ILogger<CnabParser>>();
        _mockBus = new Mock<IMediatorHandler>();
        _parser = new CnabParser(_mockLogger.Object, _mockBus.Object);
    }

    [Fact]
    public void Parse_ShouldReturnTransactions_WhenFileIsValid()
    {
        // Arrange
        var cnabLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().HaveCount(1);
        var transaction = result.First();
        transaction.Type.Should().Be(TransactionType.Financing);
        transaction.Value.Should().Be(142.00m);
        transaction.Cpf.Should().Be("09620676017");
        transaction.Card.Should().Be("4753****3153");
        transaction.StoreOwner.Should().Be("JOÃO MACEDO");
        transaction.StoreName.Should().Be("BAR DO JOÃO");
    }

    [Fact]
    public void Parse_ShouldSkipEmptyLines()
    {
        // Arrange
        var cnabData = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     \n\n3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabData);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void Parse_ShouldSkipLinesWithLessThan80Characters()
    {
        // Arrange
        var cnabData = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO\n3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabData);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void Parse_ShouldSkipTransactionWithInvalidType()
    {
        // Arrange - Type -1 is invalid
        var cnabLine = "-201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().BeEmpty();
        _mockBus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
    }

    [Fact]
    public void Parse_ShouldSkipTransactionWithZeroValue()
    {
        // Arrange - Value is 0
        var cnabLine = "3201903010000000000096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().BeEmpty();
        _mockBus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
    }

    [Fact]
    public void Parse_ShouldSkipTransactionWithEmptyCpf()
    {
        // Arrange - CPF is empty
        var cnabLine = "320190301000001420           4753****3153153453JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().BeEmpty();
        _mockBus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
    }

    [Fact]
    public void Parse_ShouldSkipTransactionWithEmptyOwner()
    {
        // Arrange - Owner is empty
        var cnabLine = "3201903010000014200096206760174753****3153153453              BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().BeEmpty();
        _mockBus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
    }

    [Fact]
    public void Parse_ShouldSkipTransactionWithEmptyStoreName()
    {
        // Arrange - Store name is empty
        var cnabLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO                                    ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().BeEmpty();
        _mockBus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
    }

    [Fact]
    public void Parse_ShouldHandleMalformedLine_AndLogError()
    {
        // Arrange - Invalid format that will cause parsing exception
        var cnabLine = "INVALID_LINE_FORMAT_THAT_WILL_CAUSE_PARSE_ERROR_AND_IS_LONG_ENOUGH_TO_PASS_LENGTH_CHECK_MINIMUM";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().BeEmpty();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
            Times.Once);
        _mockBus.Verify(b => b.RaiseEvent(It.IsAny<DomainNotification>()), Times.Once);
    }

    [Fact]
    public void Parse_ShouldParseMultipleValidTransactions()
    {
        // Arrange
        var cnabData = "1201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     \n2201903010000005000098765432104321****9876153453MARIA SILVA   LOJA DA MARIA                   \n3201903010000020000011111111114444****1111153453PEDRO SANTOS  MERCADO PEDRO                   ";
        var stream = CreateStreamFromString(cnabData);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Type.Should().Be(TransactionType.Debit);
        result[1].Type.Should().Be(TransactionType.Boleto);
        result[2].Type.Should().Be(TransactionType.Financing);
    }

    [Fact]
    public void Parse_ShouldCorrectlyDivideValueBy100()
    {
        // Arrange - Value 0000014200 should become 142.00
        var cnabLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.First().Value.Should().Be(142.00m);
    }

    [Fact]
    public void Parse_ShouldParseDateAndTimeCorrectly()
    {
        // Arrange - Date: 20190301, Time: 153453 -> 2019-03-01 15:34:53
        var cnabLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        var transaction = result.First();
        transaction.Date.Year.Should().Be(2019);
        transaction.Date.Month.Should().Be(3);
        transaction.Date.Day.Should().Be(1);
        transaction.Date.Hour.Should().Be(15);
        transaction.Date.Minute.Should().Be(34);
        transaction.Date.Second.Should().Be(53);
    }

    [Fact]
    public void Parse_ShouldHandleInvalidDateTime_AndUseUtcNow()
    {
        // Arrange - Invalid date: 99999999, Invalid time: 999999
        var cnabLine = "3999999990000014200096206760174753****3153999999JOÃO MACEDO   BAR DO JOÃO                     ";
        var stream = CreateStreamFromString(cnabLine);
        var beforeParse = DateTime.UtcNow;

        // Act
        var result = _parser.Parse(stream).ToList();
        var afterParse = DateTime.UtcNow;

        // Assert
        result.Should().HaveCount(1);
        var transaction = result.First();
        transaction.Date.Should().BeOnOrAfter(beforeParse);
        transaction.Date.Should().BeOnOrBefore(afterParse);
    }

    [Fact]
    public void Parse_ShouldTrimTrailingSpacesFromStoreName()
    {
        // Arrange - Store name has trailing spaces
        var cnabLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.First().StoreName.Should().Be("BAR DO JOÃO");
    }

    [Fact]
    public void Parse_ShouldHandleMixOfValidAndInvalidTransactions()
    {
        // Arrange
        var cnabData = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO                     \nINVALID_LINE_FORMAT_THAT_WILL_CAUSE_PARSE_ERROR_AND_IS_LONG_ENOUGH_TO_PASS_LENGTH_CHECK_MINIMUM\n4201903010000005000098765432104321****9876153453MARIA SILVA   LOJA DA MARIA                   ";
        var stream = CreateStreamFromString(cnabData);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].StoreOwner.Should().Be("JOÃO MACEDO");
        result[1].StoreOwner.Should().Be("MARIA SILVA");
    }

    [Fact]
    public void Parse_ShouldHandleExactly80CharacterLine()
    {
        // Arrange - Exactly 80 characters
        var cnabLine = "3201903010000014200096206760174753****3153153453JOÃO MACEDO   BAR DO JOÃO       ";
        cnabLine = cnabLine.PadRight(80);
        var stream = CreateStreamFromString(cnabLine);

        // Act
        var result = _parser.Parse(stream).ToList();

        // Assert
        result.Should().HaveCount(1);
    }

    private static Stream CreateStreamFromString(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }
}
