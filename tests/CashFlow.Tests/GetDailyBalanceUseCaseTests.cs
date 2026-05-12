using CashFlow.Application.Abstractions;
using CashFlow.Application.UseCases;
using CashFlow.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace CashFlow.Tests;

public sealed class GetDailyBalanceUseCaseTests
{
    [Fact]
    public async Task Should_return_null_when_daily_balance_does_not_exist()
    {
        var repo = new Mock<IDailyBalanceRepository>();
        repo.Setup(x => x.GetAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync((DailyBalance?)null);
        var useCase = new GetDailyBalanceUseCase(repo.Object);
        var response = await useCase.ExecuteAsync(new DateOnly(2025, 6, 1), CancellationToken.None);
        response.Should().BeNull();
    }

    [Fact]
    public async Task Should_map_daily_balance_response()
    {
        var balance = new DailyBalance(new DateOnly(2025, 6, 1));
        balance.Apply(200m);
        balance.Apply(-30m);
        var repo = new Mock<IDailyBalanceRepository>();
        repo.Setup(x => x.GetAsync(balance.BalanceDate, It.IsAny<CancellationToken>())).ReturnsAsync(balance);
        var useCase = new GetDailyBalanceUseCase(repo.Object);
        var response = await useCase.ExecuteAsync(balance.BalanceDate, CancellationToken.None);
        response!.Balance.Should().Be(170m);
    }
}
