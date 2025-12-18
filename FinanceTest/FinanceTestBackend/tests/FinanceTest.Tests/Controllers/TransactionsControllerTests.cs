using FinanceTest.Api.Controllers;
using FinanceTest.Application.Dtos;
using FinanceTest.Application.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTest.Tests.Controllers;

public class TransactionsControllerTests
{
    private readonly Mock<ITransactionService> _mockService;
    private readonly Mock<ILogger<TransactionsController>> _mockLogger;
    private readonly TransactionsController _controller;

    public TransactionsControllerTests()
    {
        _mockService = new Mock<ITransactionService>();
        _mockLogger = new Mock<ILogger<TransactionsController>>();
        _controller = new TransactionsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Upload_ShouldReturnOk_WhenFileIsValid()
    {
        // Arrange
        var fileMock = TransactionMock.CreateMockFile("CNAB.txt", 100);
        
        _mockService.Setup(s => s.ProcessFileAsync(It.IsAny<Stream>()))
                    .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Upload(fileMock.Object);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().BeEquivalentTo(new { message = "File processed and transactions saved successfully." });

        _mockService.Verify(s => s.ProcessFileAsync(It.IsAny<Stream>()), Times.Once);
    }

    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenFileIsEmpty()
    {
        // Arrange
        var fileMock = TransactionMock.CreateMockFile("CNAB.txt", 0);

        // Act
        var result = await _controller.Upload(fileMock.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
              .Which.Value.Should().BeEquivalentTo(new { message = "No file was uploaded." });

        _mockService.Verify(s => s.ProcessFileAsync(It.IsAny<Stream>()), Times.Never);
    }

    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenFileExtensionIsInvalid()
    {
        // Arrange
        var fileMock = TransactionMock.CreateMockFile("image.png", 100);

        // Act
        var result = await _controller.Upload(fileMock.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
              .Which.Value.Should().BeEquivalentTo(new { message = "Invalid file format. Please upload a .txt file." });

        _mockService.Verify(s => s.ProcessFileAsync(It.IsAny<Stream>()), Times.Never);
    }

    [Fact]
    public async Task Upload_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
    {
        // Arrange
        var fileMock = TransactionMock.CreateMockFile("CNAB.txt", 100);
        const string expectedErrorMessage = "Invalid data format in line 10.";

        _mockService.Setup(s => s.ProcessFileAsync(It.IsAny<Stream>()))
                    .ThrowsAsync(new ArgumentException(expectedErrorMessage));

        // Act
        var result = await _controller.Upload(fileMock.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
              .Which.Value.Should().BeEquivalentTo(new { message = expectedErrorMessage });

        VerifyLoggerWasCalled(LogLevel.Warning);
    }

    [Fact]
    public async Task Upload_ShouldReturnInternalServerError_WhenServiceThrowsGenericException()
    {
        // Arrange
        var fileMock = TransactionMock.CreateMockFile("CNAB.txt", 100);

        _mockService.Setup(s => s.ProcessFileAsync(It.IsAny<Stream>()))
                    .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.Upload(fileMock.Object);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeEquivalentTo(new { message = "An internal error occurred while processing the file." });

        VerifyLoggerWasCalled(LogLevel.Error);
    }

    [Fact]
    public async Task GetStoreBalances_ShouldReturnOk_WithListOfBalances()
    {
        // Arrange
        var expectedData = new List<StoreBalanceDto>
        {
            new() { StoreName = "Store A", TotalBalance = 100 },
            new() { StoreName = "Store B", TotalBalance = -50 }
        };

        _mockService.Setup(s => s.GetStoreBalancesAsync())
                    .ReturnsAsync(expectedData);

        // Act
        var result = await _controller.GetStoreBalances();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnData = okResult.Value.Should().BeAssignableTo<List<StoreBalanceDto>>().Subject;

        returnData.Should().HaveCount(2);
        returnData.First().StoreName.Should().Be("Store A");
        returnData.Last().TotalBalance.Should().Be(-50);
    }

    [Fact]
    public async Task GetStoreBalances_ShouldReturnOk_WhenListIsEmpty()
    {
        // Arrange
        _mockService.Setup(s => s.GetStoreBalancesAsync()).ReturnsAsync(new List<StoreBalanceDto>());

        // Act
        var result = await _controller.GetStoreBalances();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnData = okResult.Value.Should().BeAssignableTo<List<StoreBalanceDto>>().Subject;

        returnData.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStoreBalances_ShouldReturnInternalServerError_WhenServiceFails()
    {
        // Arrange
        _mockService.Setup(s => s.GetStoreBalancesAsync())
                    .ThrowsAsync(new Exception("Database timeout"));

        // Act
        var result = await _controller.GetStoreBalances();

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeEquivalentTo(new { message = "Error retrieving data." });

        VerifyLoggerWasCalled(LogLevel.Error);
    }
    
    private void VerifyLoggerWasCalled(LogLevel level)
    {
        _mockLogger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
    }
}