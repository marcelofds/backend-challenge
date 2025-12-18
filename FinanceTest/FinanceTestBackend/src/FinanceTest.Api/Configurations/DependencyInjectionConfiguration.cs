using System.Diagnostics.CodeAnalysis;
using FinanceTest.Application.Interfaces;
using FinanceTest.Application.Services;
using FinanceTest.Application.Services.Interfaces;
using FinanceTest.Domain.Entities.Transactions.Repositories;
using FinanceTest.Domain.Notifications.Bus;
using FinanceTest.Domain.Notifications.Events;
using FinanceTest.Domain.Notifications.Handlers;
using FinanceTest.Infrastructure.Parsers;
using FinanceTest.Infrastructure.Repositories;
using MediatR;

namespace FinanceTest.Api.Configurations;
[ExcludeFromCodeCoverage]
public static class DependencyInjectionConfiguration
{
    public static void AddDependencyInjection(this IServiceCollection services)
    {
        // Bus
        services.AddScoped<IMediatorHandler, InMemoryBus>();

        // Events
        services.AddScoped<DomainNotificationHandler>();
        services.AddScoped<INotificationHandler<DomainNotification>>(x =>
            x.GetRequiredService<DomainNotificationHandler>());

        //Parser
        services.AddScoped<IFileParser, CnabParser>();

        //Repositories
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        //Services
        services.AddScoped<ITransactionService, TransactionService>();
    }
}