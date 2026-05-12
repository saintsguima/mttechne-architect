using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Data;

public sealed class CashFlowDbContext : DbContext
{
    public CashFlowDbContext(DbContextOptions<CashFlowDbContext> options) : base(options) { }

    public DbSet<CashEntry> CashEntries => Set<CashEntry>();
    public DbSet<DailyBalance> DailyBalances => Set<DailyBalance>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CashEntry>(builder =>
        {
            builder.ToTable("CashEntry");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.EntryDate).HasColumnType("date").IsRequired();
            builder.Property(x => x.Type).HasConversion<int>().IsRequired();
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.HasIndex(x => x.EntryDate);
        });

        modelBuilder.Entity<DailyBalance>(builder =>
        {
            builder.ToTable("DailyBalance");
            builder.HasKey(x => x.BalanceDate);
            builder.Property(x => x.BalanceDate).HasColumnType("date");
            builder.Property(x => x.TotalCredits).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalDebits).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Balance).HasColumnType("decimal(18,2)");
            builder.Property(x => x.UpdatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.ToTable("OutboxMessage");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Payload).IsRequired();
            builder.Property(x => x.LastError).HasMaxLength(1000);
            builder.HasIndex(x => new { x.ProcessedAtUtc, x.CreatedAtUtc });
        });
    }
}
