using System.Text.Json;
using CashFlow.Application.Abstractions;
using CashFlow.Application.DTOs;
using CashFlow.Domain.Entities;

namespace CashFlow.Application.UseCases;

public sealed class CreateCashEntryUseCase
{
    private readonly ICashEntryRepository _cashEntryRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCashEntryUseCase(ICashEntryRepository cashEntryRepository, IOutboxRepository outboxRepository, IUnitOfWork unitOfWork)
    {
        _cashEntryRepository = cashEntryRepository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CashEntryResponse> ExecuteAsync(CreateCashEntryRequest request, CancellationToken cancellationToken)
    {
        var entry = new CashEntry(request.EntryDate, request.Type, request.Amount, request.Description);
        var evt = new CashEntryCreatedEvent(entry.Id, entry.EntryDate, entry.Type, entry.Amount, entry.SignedAmount, entry.CreatedAtUtc);
        var outbox = new OutboxMessage(nameof(CashEntryCreatedEvent), JsonSerializer.Serialize(evt));

        await _cashEntryRepository.AddAsync(entry, cancellationToken);
        await _outboxRepository.AddAsync(outbox, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new CashEntryResponse(entry.Id, entry.EntryDate, entry.Type, entry.Amount, entry.Description, entry.CreatedAtUtc);
    }
}
