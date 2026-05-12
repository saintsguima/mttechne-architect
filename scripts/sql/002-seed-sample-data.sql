USE CashFlowDb;
GO

INSERT INTO dbo.CashEntry (Id, EntryDate, Type, Amount, Description, CreatedAtUtc)
VALUES
(NEWID(), '2025-06-01', 1, 1000.00, 'Venda no balcão', SYSUTCDATETIME()),
(NEWID(), '2025-06-01', 2, 250.00, 'Pagamento fornecedor', SYSUTCDATETIME()),
(NEWID(), '2025-06-02', 1, 300.00, 'Recebimento PIX', SYSUTCDATETIME());
GO
