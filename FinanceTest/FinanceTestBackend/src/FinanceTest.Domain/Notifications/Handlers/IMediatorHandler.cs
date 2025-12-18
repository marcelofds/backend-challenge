using FinanceTest.Domain.Notifications.Events;

namespace FinanceTest.Domain.Notifications.Handlers;

public interface IMediatorHandler
{
    Task RaiseEvent<T>(T @event) where T : Event;
}