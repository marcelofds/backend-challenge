using FinanceTest.Application.Dtos;

namespace FinanceTest.Application.Services.Interfaces;

public interface ITransactionService
{
    Task ProcessFileAsync(Stream fileStream);
    Task<List<StoreBalanceDto>> GetStoreBalancesAsync();
}