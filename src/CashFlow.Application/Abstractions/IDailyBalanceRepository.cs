using CashFlow.Domain.Entities;

namespace CashFlow.Application.Abstractions;

public interface IDailyBalanceRepository
{
    Task<DailyBalance?> GetAsync(DateOnly date, CancellationToken cancellationToken);
    Task UpsertByEntryAsync(DateOnly date, decimal signedAmount, CancellationToken cancellationToken);
}
