using FinanceTest.Domain.Enums;
using FluentValidation.Results;

namespace FinanceTest.Domain.Entities.Transactions;

public class Transaction
{
    public Guid Id { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime Date { get; private set; }
    public decimal Value { get; private set; }
    public string Cpf { get; private set; } = null!;
    public string Card { get; private set; } = null!;
    public string StoreOwner { get; private set; } = null!;
    public string StoreName { get; private set; } = null!;

    protected Transaction()
    {
    }

    public Transaction(int type, DateTime date, decimal value, string cpf, string card, string storeOwner,
        string storeName)
    {
        Id = Guid.NewGuid();
        Type = (TransactionType)type;
        Date = date;
        Value = value;
        Cpf = cpf;
        Card = card;
        StoreOwner = storeOwner.Trim();
        StoreName = storeName.Trim();
    }

    public decimal GetSignedValue()
    {
        return Type switch
        {
            TransactionType.Boleto or
                TransactionType.Financing or
                TransactionType.Rent => -Value,
            _ => Value
        };
    }
    
    public ValidationResult Validate() 
        => new TransactionValidator().Validate(this);
}