namespace CashFlow.Domain.Entities;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    public int Attempts { get; private set; }
    public string? LastError { get; private set; }

    private OutboxMessage() { }

    public OutboxMessage(string type, string payload)
    {
        Id = Guid.NewGuid();
        Type = type;
        Payload = payload;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void MarkProcessed() => ProcessedAtUtc = DateTime.UtcNow;

    public void MarkFailed(string error)
    {
        Attempts++;
        LastError = error.Length > 1000 ? error[..1000] : error;
    }
}
