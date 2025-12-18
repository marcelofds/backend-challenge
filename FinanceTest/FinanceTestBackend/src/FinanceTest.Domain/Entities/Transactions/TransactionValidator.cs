using FluentValidation;

namespace FinanceTest.Domain.Entities.Transactions;

public class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid transaction type.");

        RuleFor(x => x.Value)
            .GreaterThan(0)
            .WithMessage("Transaction value must be greater than zero.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required.")
            .Length(11).WithMessage("CPF must be exactly 11 characters.")
            .Matches(@"^\d+$").WithMessage("CPF must contain only numbers.");

        RuleFor(x => x.Card)
            .NotEmpty().WithMessage("Card number is required.")
            .Length(12).WithMessage("Card number must be exactly 12 characters.");

        RuleFor(x => x.StoreOwner)
            .NotEmpty().WithMessage("Store owner name is required.")
            .MaximumLength(14).WithMessage("Store owner name must be 14 characters or fewer.");

        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Store name is required.")
            .MaximumLength(19).WithMessage("Store name must be 19 characters or fewer.");
    }
}