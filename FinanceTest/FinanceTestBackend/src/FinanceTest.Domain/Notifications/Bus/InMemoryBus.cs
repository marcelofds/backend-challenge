using FinanceTest.Domain.Notifications.Events;
using FinanceTest.Domain.Notifications.Handlers;
using MediatR;

namespace FinanceTest.Domain.Notifications.Bus;

public sealed class InMemoryBus : IMediatorHandler
{
    private readonly IMediator _mediator;

    public InMemoryBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task RaiseEvent<T>(T @event) where T : Event => _mediator.Publish(@event);
}