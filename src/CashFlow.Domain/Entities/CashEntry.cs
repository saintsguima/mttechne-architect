using CashFlow.Domain.Enums;
using CashFlow.Domain.Exceptions;

namespace CashFlow.Domain.Entities;

public sealed class CashEntry
{
    public Guid Id { get; private set; }
    public DateOnly EntryDate { get; private set; }
    public EntryType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    private CashEntry() { }

    public CashEntry(DateOnly entryDate, EntryType type, decimal amount, string description)
    {
        if (amount <= 0) throw new DomainException("O valor do lançamento deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("A descrição do lançamento é obrigatória.");
        if (description.Length > 200) throw new DomainException("A descrição deve possuir no máximo 200 caracteres.");

        Id = Guid.NewGuid();
        EntryDate = entryDate;
        Type = type;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Description = description.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public decimal SignedAmount => Type == EntryType.Credit ? Amount : -Amount;
}
