using System.Net;
using FinanceTest.Api.Filters;
using FinanceTest.Domain.Notifications.Events;
using FinanceTest.Domain.Notifications.Handlers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTest.Tests.Filters;

public class NotificationFilterTests
{
    private readonly Mock<ILogger<DomainNotificationHandler>> _mockLogger;
    private readonly DomainNotificationHandler _notificationHandler;
    private readonly NotificationFilter _filter;

    public NotificationFilterTests()
    {
        _mockLogger = new Mock<ILogger<DomainNotificationHandler>>();
        _notificationHandler = new DomainNotificationHandler(_mockLogger.Object);
        _filter = new NotificationFilter(_notificationHandler);
    }

    [Fact]
    public async Task OnResultExecutionAsync_ShouldReturnBadRequest_WhenNotificationsExist()
    {
        // Arrange
        var notification1 = new DomainNotification("Error1", "First error message");
        var notification2 = new DomainNotification("Error2", "Second error message");

        await _notificationHandler.Handle(notification1, CancellationToken.None);
        await _notificationHandler.Handle(notification2, CancellationToken.None);

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor()
        );

        var resultExecutingContext = new ResultExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new OkResult(),
            new object()
        );

        var nextCalled = false;
        ResultExecutionDelegate next = () =>
        {
            nextCalled = true;
            return Task.FromResult(new ResultExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                new OkResult(),
                new object()
            ));
        };

        // Act
        await _filter.OnResultExecutionAsync(resultExecutingContext, next);

        // Assert
        httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        httpContext.Response.ContentType.Should().StartWith("application/json");
        nextCalled.Should().BeFalse();

        // Read the response body
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        responseBody.Should().Contain("Error1");
        responseBody.Should().Contain("Error2");
        responseBody.Should().Contain("First error message");
        responseBody.Should().Contain("Second error message");
    }

    [Fact]
    public async Task OnResultExecutionAsync_ShouldCallNext_WhenNoNotifications()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor()
        );

        var resultExecutingContext = new ResultExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new OkResult(),
            new object()
        );

        var nextCalled = false;
        ResultExecutionDelegate next = () =>
        {
            nextCalled = true;
            return Task.FromResult(new ResultExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                new OkResult(),
                new object()
            ));
        };

        // Act
        await _filter.OnResultExecutionAsync(resultExecutingContext, next);

        // Assert
        nextCalled.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be(200); // Default OK status
    }

    [Fact]
    public async Task OnResultExecutionAsync_ShouldFormatNotificationsCorrectly()
    {
        // Arrange
        var notification = new DomainNotification("ValidationError", "Invalid data provided");
        await _notificationHandler.Handle(notification, CancellationToken.None);

        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor()
        );

        var resultExecutingContext = new ResultExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new OkResult(),
            new object()
        );

        ResultExecutionDelegate next = () => Task.FromResult(new ResultExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            new OkResult(),
            new object()
        ));

        // Act
        await _filter.OnResultExecutionAsync(resultExecutingContext, next);

        // Assert
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        responseBody.Should().Contain("\"errors\"");
        responseBody.Should().Contain("\"key\""); // JSON serializer uses camelCase
        responseBody.Should().Contain("\"value\""); // JSON serializer uses camelCase
        responseBody.Should().Contain("ValidationError");
        responseBody.Should().Contain("Invalid data provided");
    }
}
