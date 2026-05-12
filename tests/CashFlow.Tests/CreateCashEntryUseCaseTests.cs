using CashFlow.Application.Abstractions;
using CashFlow.Application.DTOs;
using CashFlow.Application.UseCases;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace CashFlow.Tests;

public sealed class CreateCashEntryUseCaseTests
{
    [Fact]
    public async Task Should_create_entry_and_outbox_message_in_same_transaction()
    {
        var entryRepo = new Mock<ICashEntryRepository>();
        var outboxRepo = new Mock<IOutboxRepository>();
        var uow = new Mock<IUnitOfWork>();
        var useCase = new CreateCashEntryUseCase(entryRepo.Object, outboxRepo.Object, uow.Object);

        var request = new CreateCashEntryRequest(new DateOnly(2025, 6, 1), EntryType.Credit, 99.90m, "Venda balcão");
        var response = await useCase.ExecuteAsync(request, CancellationToken.None);

        response.Amount.Should().Be(99.90m);
        response.Type.Should().Be(EntryType.Credit);
        entryRepo.Verify(x => x.AddAsync(It.IsAny<CashEntry>(), It.IsAny<CancellationToken>()), Times.Once);
        outboxRepo.Verify(x => x.AddAsync(It.IsAny<OutboxMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
