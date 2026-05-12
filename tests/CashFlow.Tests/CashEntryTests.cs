using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CashFlow.Tests;

public sealed class CashEntryTests
{
    [Fact]
    public void Should_create_credit_entry_with_signed_positive_amount()
    {
        var entry = new CashEntry(new DateOnly(2025, 6, 1), EntryType.Credit, 100.555m, "Venda");
        entry.Amount.Should().Be(100.56m);
        entry.SignedAmount.Should().Be(100.56m);
    }

    [Fact]
    public void Should_create_debit_entry_with_signed_negative_amount()
    {
        var entry = new CashEntry(new DateOnly(2025, 6, 1), EntryType.Debit, 40m, "Compra");
        entry.SignedAmount.Should().Be(-40m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Should_reject_invalid_amount(decimal amount)
    {
        Action act = () => new CashEntry(DateOnly.FromDateTime(DateTime.Today), EntryType.Credit, amount, "Teste");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_reject_empty_description()
    {
        Action act = () => new CashEntry(DateOnly.FromDateTime(DateTime.Today), EntryType.Credit, 10, " ");
        act.Should().Throw<DomainException>();
    }
}
