using MediatR;

namespace FinanceTest.Domain.Notifications.Events;

public abstract class Event : Message, INotification
{
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
}