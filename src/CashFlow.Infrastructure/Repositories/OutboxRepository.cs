using CashFlow.Application.Abstractions;
using CashFlow.Domain.Entities;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories;

public sealed class OutboxRepository : IOutboxRepository
{
    private readonly CashFlowDbContext _context;
    public OutboxRepository(CashFlowDbContext context) => _context = context;

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken) =>
        await _context.OutboxMessages.AddAsync(message, cancellationToken);

    public async Task<IReadOnlyCollection<OutboxMessage>> GetPendingAsync(int take, CancellationToken cancellationToken) =>
        await _context.OutboxMessages
            .Where(x => x.ProcessedAtUtc == null && x.Attempts < 5)
            .OrderBy(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);

    public async Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken)
    {
        var message = await _context.OutboxMessages.FirstAsync(x => x.Id == id, cancellationToken);
        message.MarkProcessed();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkFailedAsync(Guid id, string error, CancellationToken cancellationToken)
    {
        var message = await _context.OutboxMessages.FirstAsync(x => x.Id == id, cancellationToken);
        message.MarkFailed(error);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
