using FinanceTest.Domain.Notifications.Bus;
using FinanceTest.Domain.Notifications.Events;
using FluentAssertions;
using MediatR;
using Moq;

namespace FinanceTest.Tests.Notifications;

public class InMemoryBusTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly InMemoryBus _bus;

    public InMemoryBusTests()
    {
        _mockMediator = new Mock<IMediator>();
        _bus = new InMemoryBus(_mockMediator.Object);
    }

    [Fact]
    public async Task RaiseEvent_ShouldPublishEventToMediator()
    {
        // Arrange
        var domainEvent = new DomainNotification("TestKey", "TestValue");
        _mockMediator.Setup(m => m.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bus.RaiseEvent(domainEvent);

        // Assert
        _mockMediator.Verify(
            m => m.Publish(domainEvent, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RaiseEvent_ShouldHandleMultipleEvents()
    {
        // Arrange
        var event1 = new DomainNotification("Key1", "Value1");
        var event2 = new DomainNotification("Key2", "Value2");
        var event3 = new DomainNotification("Key3", "Value3");

        _mockMediator.Setup(m => m.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _bus.RaiseEvent(event1);
        await _bus.RaiseEvent(event2);
        await _bus.RaiseEvent(event3);

        // Assert
        _mockMediator.Verify(
            m => m.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }
}
