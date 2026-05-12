using CashFlow.Domain.Entities;

namespace CashFlow.Application.Abstractions;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int take, CancellationToken cancellationToken);
    Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken);
    Task MarkFailedAsync(Guid id, string error, CancellationToken cancellationToken);
}
