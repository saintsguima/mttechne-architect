using CashFlow.Domain.Entities;

namespace CashFlow.Application.Abstractions;

public interface ICashEntryRepository
{
    Task AddAsync(CashEntry entry, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CashEntry>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
}
