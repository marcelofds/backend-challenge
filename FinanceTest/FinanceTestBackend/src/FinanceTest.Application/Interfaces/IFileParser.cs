using FinanceTest.Domain.Entities;
using FinanceTest.Domain.Entities.Transactions;

namespace FinanceTest.Application.Interfaces;

public interface IFileParser
{
    IEnumerable<Transaction> Parse(Stream fileStream);
}