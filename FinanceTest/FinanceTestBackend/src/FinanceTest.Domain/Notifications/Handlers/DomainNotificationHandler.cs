using FinanceTest.Domain.Notifications.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FinanceTest.Domain.Notifications.Handlers;

public class DomainNotificationHandler : INotificationHandler<DomainNotification>
{
    private readonly ILogger<DomainNotificationHandler> _logger;
    private List<DomainNotification> _notifications;

    public DomainNotificationHandler(ILogger<DomainNotificationHandler> logger)
    {
        _logger = logger;
        _notifications = new ();
    }

    public Task Handle(DomainNotification message, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Validation: {MessageKey} - {MessageValue}", message.Key, message.Value);
            
        _notifications.Add(message);
        return Task.CompletedTask;
    }

    public IEnumerable<DomainNotification> GetNotifications()
    {
        return _notifications;
    }

    public bool HasNotifications()
    {
        return GetNotifications().Any();
    }

    public void Dispose()
    {
        _notifications = new ();
    }
}