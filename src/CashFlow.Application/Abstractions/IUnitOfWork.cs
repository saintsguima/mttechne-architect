namespace CashFlow.Application.Abstractions;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken);
}
