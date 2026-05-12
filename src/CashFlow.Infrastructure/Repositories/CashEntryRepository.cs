using CashFlow.Application.Abstractions;
using CashFlow.Domain.Entities;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories;

public sealed class CashEntryRepository : ICashEntryRepository
{
    private readonly CashFlowDbContext _context;
    public CashEntryRepository(CashFlowDbContext context) => _context = context;

    public async Task AddAsync(CashEntry entry, CancellationToken cancellationToken) =>
        await _context.CashEntries.AddAsync(entry, cancellationToken);

    public async Task<IReadOnlyCollection<CashEntry>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken) =>
        await _context.CashEntries.AsNoTracking().Where(x => x.EntryDate == date).ToListAsync(cancellationToken);
}
