using FinanceTest.Application.Dtos;
using FinanceTest.Application.Interfaces;
using FinanceTest.Application.Services.Interfaces;
using FinanceTest.Domain.Entities.Transactions.Repositories;

namespace FinanceTest.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly IFileParser _parser;

    public TransactionService(ITransactionRepository repository, IFileParser parser)
    {
        _repository = repository;
        _parser = parser;
    }

    public async Task ProcessFileAsync(Stream fileStream)
    {
        var transactions = _parser.Parse(fileStream).ToList();

        if (transactions.Count != 0)
            await _repository.AddRangeAsync(transactions);
    }

    public async Task<List<StoreBalanceDto>> GetStoreBalancesAsync()
    {
        var transactions = await _repository.GetAllAsync();
        
        var result = transactions
            .GroupBy(t => t.StoreName)
            .Select(g => new StoreBalanceDto
            {
                StoreName = g.Key,
                Operations = g.Select(t => new OperationDto(
                    t.Type, 
                    t.Date, 
                    t.Value, 
                    t.Cpf, 
                    t.Card,
                    t.GetSignedValue()
                )).ToList(),
                TotalBalance = g.Sum(t => t.GetSignedValue())
            })
            .ToList();

        return result;
    }
}