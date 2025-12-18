using FinanceTest.Domain.Enums;

namespace FinanceTest.Application.Dtos;

public record OperationDto(
    TransactionType Type,
    DateTime Date,
    decimal Value,
    string Cpf,
    string Card,
    decimal SignedValue
);