namespace FinanceTest.Application.Dtos;

public record StoreBalanceDto
{
    public string StoreName { get; init; } = string.Empty;
    public decimal TotalBalance { get; init; }
    public List<OperationDto> Operations { get; init; } = new ();
}