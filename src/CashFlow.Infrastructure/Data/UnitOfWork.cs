using CashFlow.Application.Abstractions;

namespace CashFlow.Infrastructure.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly CashFlowDbContext _context;
    public UnitOfWork(CashFlowDbContext context) => _context = context;
    public Task CommitAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
}
