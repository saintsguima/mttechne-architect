using CashFlow.Domain.Enums;

namespace CashFlow.Application.DTOs;

public sealed record CreateCashEntryRequest(DateOnly EntryDate, EntryType Type, decimal Amount, string Description);
public sealed record CashEntryResponse(Guid Id, DateOnly EntryDate, EntryType Type, decimal Amount, string Description, DateTime CreatedAtUtc);
public sealed record DailyBalanceResponse(DateOnly BalanceDate, decimal TotalCredits, decimal TotalDebits, decimal Balance, DateTime UpdatedAtUtc);
public sealed record CashEntryCreatedEvent(Guid Id, DateOnly EntryDate, EntryType Type, decimal Amount, decimal SignedAmount, DateTime CreatedAtUtc);
