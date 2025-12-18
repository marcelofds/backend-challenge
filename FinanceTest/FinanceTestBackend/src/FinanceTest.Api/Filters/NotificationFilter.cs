using System.Net;
using FinanceTest.Domain.Notifications.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinanceTest.Api.Filters;

public class NotificationFilter : IAsyncResultFilter
{
    private readonly DomainNotificationHandler _notificationHandler;

    public NotificationFilter(DomainNotificationHandler notificationHandler)
    {
        _notificationHandler = notificationHandler;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (_notificationHandler.HasNotifications())
        {
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.HttpContext.Response.ContentType = "application/json";

            var notifications = _notificationHandler.GetNotifications();
            var response = new { errors = notifications.Select(n => new { n.Key, n.Value }) };
            
            await context.HttpContext.Response.WriteAsJsonAsync(response);

            return;
        }

        await next();
    }
}