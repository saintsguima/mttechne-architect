using CashFlow.Application.Abstractions;
using CashFlow.Domain.Entities;
using CashFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories;

public sealed class DailyBalanceRepository : IDailyBalanceRepository
{
    private readonly CashFlowDbContext _context;
    public DailyBalanceRepository(CashFlowDbContext context) => _context = context;

    public Task<DailyBalance?> GetAsync(DateOnly date, CancellationToken cancellationToken) =>
        _context.DailyBalances.AsNoTracking().FirstOrDefaultAsync(x => x.BalanceDate == date, cancellationToken);

    public async Task UpsertByEntryAsync(DateOnly date, decimal signedAmount, CancellationToken cancellationToken)
    {
        var balance = await _context.DailyBalances.FirstOrDefaultAsync(x => x.BalanceDate == date, cancellationToken);
        if (balance is null)
        {
            balance = new DailyBalance(date);
            await _context.DailyBalances.AddAsync(balance, cancellationToken);
        }
        balance.Apply(signedAmount);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
