namespace FinanceTest.Domain.Entities.Transactions.Repositories;

public interface ITransactionRepository
{
    Task AddRangeAsync(IEnumerable<Transaction> transactions);
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<IEnumerable<Transaction>> GetByStoreAsync(string storeName);
}