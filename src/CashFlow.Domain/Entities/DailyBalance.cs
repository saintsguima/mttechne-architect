namespace CashFlow.Domain.Entities;

public sealed class DailyBalance
{
    public DateOnly BalanceDate { get; private set; }
    public decimal TotalCredits { get; private set; }
    public decimal TotalDebits { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private DailyBalance() { }

    public DailyBalance(DateOnly balanceDate)
    {
        BalanceDate = balanceDate;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Apply(decimal signedAmount)
    {
        if (signedAmount >= 0) TotalCredits += signedAmount;
        else TotalDebits += Math.Abs(signedAmount);

        Balance = TotalCredits - TotalDebits;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
