using FinanceTest.Domain.Notifications.Events;
using FinanceTest.Domain.Notifications.Handlers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTest.Tests.Notifications;

public class DomainNotificationHandlerTests
{
    private readonly Mock<ILogger<DomainNotificationHandler>> _mockLogger;
    private readonly DomainNotificationHandler _handler;

    public DomainNotificationHandlerTests()
    {
        _mockLogger = new Mock<ILogger<DomainNotificationHandler>>();
        _handler = new DomainNotificationHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddNotification_AndLogWarning()
    {
        // Arrange
        var notification = new DomainNotification("TestKey", "TestValue");

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _handler.GetNotifications().Should().Contain(notification);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldAddMultipleNotifications()
    {
        // Arrange
        var notification1 = new DomainNotification("Key1", "Value1");
        var notification2 = new DomainNotification("Key2", "Value2");
        var notification3 = new DomainNotification("Key3", "Value3");

        // Act
        await _handler.Handle(notification1, CancellationToken.None);
        await _handler.Handle(notification2, CancellationToken.None);
        await _handler.Handle(notification3, CancellationToken.None);

        // Assert
        var notifications = _handler.GetNotifications().ToList();
        notifications.Should().HaveCount(3);
        notifications.Should().Contain(notification1);
        notifications.Should().Contain(notification2);
        notifications.Should().Contain(notification3);
    }

    [Fact]
    public void GetNotifications_ShouldReturnEmpty_WhenNoNotificationsAdded()
    {
        // Act
        var result = _handler.GetNotifications();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnAllNotifications()
    {
        // Arrange
        var notification1 = new DomainNotification("Key1", "Value1");
        var notification2 = new DomainNotification("Key2", "Value2");
        await _handler.Handle(notification1, CancellationToken.None);
        await _handler.Handle(notification2, CancellationToken.None);

        // Act
        var result = _handler.GetNotifications();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void HasNotifications_ShouldReturnFalse_WhenNoNotifications()
    {
        // Act
        var result = _handler.HasNotifications();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasNotifications_ShouldReturnTrue_WhenNotificationsExist()
    {
        // Arrange
        var notification = new DomainNotification("TestKey", "TestValue");
        await _handler.Handle(notification, CancellationToken.None);

        // Act
        var result = _handler.HasNotifications();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Dispose_ShouldClearAllNotifications()
    {
        // Arrange
        var notification1 = new DomainNotification("Key1", "Value1");
        var notification2 = new DomainNotification("Key2", "Value2");
        await _handler.Handle(notification1, CancellationToken.None);
        await _handler.Handle(notification2, CancellationToken.None);

        // Act
        _handler.Dispose();

        // Assert
        _handler.GetNotifications().Should().BeEmpty();
        _handler.HasNotifications().Should().BeFalse();
    }
}
