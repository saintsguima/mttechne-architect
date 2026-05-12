using CashFlow.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CashFlow.Tests;

public sealed class DailyBalanceTests
{
    [Fact]
    public void Should_apply_credits_and_debits()
    {
        var balance = new DailyBalance(new DateOnly(2025, 6, 1));
        balance.Apply(150m);
        balance.Apply(-50m);
        balance.TotalCredits.Should().Be(150m);
        balance.TotalDebits.Should().Be(50m);
        balance.Balance.Should().Be(100m);
    }
}
