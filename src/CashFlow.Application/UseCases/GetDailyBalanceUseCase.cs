using CashFlow.Application.Abstractions;
using CashFlow.Application.DTOs;

namespace CashFlow.Application.UseCases;

public sealed class GetDailyBalanceUseCase
{
    private readonly IDailyBalanceRepository _repository;

    public GetDailyBalanceUseCase(IDailyBalanceRepository repository) => _repository = repository;

    public async Task<DailyBalanceResponse?> ExecuteAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var balance = await _repository.GetAsync(date, cancellationToken);
        return balance is null
            ? null
            : new DailyBalanceResponse(balance.BalanceDate, balance.TotalCredits, balance.TotalDebits, balance.Balance, balance.UpdatedAtUtc);
    }
}
