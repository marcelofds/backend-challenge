using FinanceTest.Domain.Entities.Transactions;
using FinanceTest.Domain.Entities.Transactions.Repositories;
using FinanceTest.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceTest.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly FinanceTestContext _context;

    // Injeção de Dependência do Contexto do Banco de Dados
    public TransactionRepository(FinanceTestContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions)
    {
        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        return await _context.Transactions
            .AsNoTracking()
            .OrderBy(t => t.StoreName)
            .ThenBy(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByStoreAsync(string storeName)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.StoreName == storeName)
            .ToListAsync();
    }
}