using System.Globalization;
using FinanceTest.Application.Interfaces;
using FinanceTest.Domain.Entities.Transactions;
using FinanceTest.Domain.Notifications.Events;
using FinanceTest.Domain.Notifications.Handlers;
using Microsoft.Extensions.Logging;

namespace FinanceTest.Infrastructure.Parsers;

public class CnabParser : IFileParser
{
    private readonly ILogger<CnabParser> _logger;
    private readonly IMediatorHandler _bus;

    public CnabParser(ILogger<CnabParser> logger, IMediatorHandler bus)
    {
        _logger = logger;
        _bus = bus;
    }
    
    public IEnumerable<Transaction> Parse(Stream fileStream)
    {
        var transactions = new List<Transaction>();
        using var reader = new StreamReader(fileStream);
        var lineNumber = 0;

        while (!reader.EndOfStream)
        {
            lineNumber++;
            var line = reader.ReadLine();
            
            if (string.IsNullOrWhiteSpace(line) || line.Length < 80) continue;

            try
            {
                // Type: Pos 1, Len 1 -> Index 0
                var type = int.Parse(line.Substring(0, 1));

                // Date: Pos 2, Len 8 -> Index 1
                var dateStr = line.Substring(1, 8);

                // Value: Pos 10, Len 10 -> Index 9
                var valueStr = line.Substring(9, 10);

                // CPF: Pos 20, Len 11 -> Index 19
                var cpf = line.Substring(19, 11);

                // Card: Pos 31, Len 12 -> Index 30
                var card = line.Substring(30, 12);

                // Time: Pos 43, Len 6 -> Index 42
                var timeStr = line.Substring(42, 6);

                // Owner: Pos 49, Len 14 -> Index 48
                var owner = line.Substring(48, 14);

                // Store: Pos 63, Len 19 -> Index 62
                // SAFETY FIX: Instead of fixed (62, 19), we take from 62 until the end.
                // This prevents ArgumentOutOfRangeException if the line is exactly 80 chars long.
                var store = line[62..].TrimEnd();

                // Normalization
                var value = decimal.Parse(valueStr) / 100m;

                // Combine Date (YYYYMMDD) and Time (HHmmss)
                if (!DateTime.TryParseExact($"{dateStr}{timeStr}", "yyyyMMddHHmmss",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                {
                    // Fallback to UTC now if parsing fails (or handle as error)
                    dateTime = DateTime.UtcNow;
                }

                var transaction = new Transaction(type, dateTime, value, cpf, card, owner, store);
                var validationResult = transaction.Validate();

                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        _bus.RaiseEvent(new DomainNotification($"Line {lineNumber}: {error.PropertyName}", error.ErrorMessage));
                    }
                    continue; 
                }
                
                transactions.Add(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _bus.RaiseEvent(new DomainNotification($"Line {lineNumber}", "Invalid data format in line."));
            }
        }

        return transactions;
    }
}