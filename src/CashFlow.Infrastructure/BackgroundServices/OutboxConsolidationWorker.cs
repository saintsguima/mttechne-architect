using System.Text.Json;
using CashFlow.Application.Abstractions;
using CashFlow.Application.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CashFlow.Infrastructure.BackgroundServices;

public sealed class OutboxConsolidationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxConsolidationWorker> _logger;

    public OutboxConsolidationWorker(IServiceScopeFactory scopeFactory, ILogger<OutboxConsolidationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var outbox = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
            var balances = scope.ServiceProvider.GetRequiredService<IDailyBalanceRepository>();
            var messages = await outbox.GetPendingAsync(100, stoppingToken);

            foreach (var message in messages)
            {
                try
                {
                    var evt = JsonSerializer.Deserialize<CashEntryCreatedEvent>(message.Payload)
                        ?? throw new InvalidOperationException("Payload inválido.");
                    await balances.UpsertByEntryAsync(evt.EntryDate, evt.SignedAmount, stoppingToken);
                    await outbox.MarkProcessedAsync(message.Id, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consolidar mensagem {MessageId}", message.Id);
                    await outbox.MarkFailedAsync(message.Id, ex.Message, stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}
