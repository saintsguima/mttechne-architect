IF DB_ID('CashFlowDb') IS NULL
BEGIN
    CREATE DATABASE CashFlowDb;
END
GO

USE CashFlowDb;
GO

IF OBJECT_ID('dbo.CashEntry', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CashEntry
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_CashEntry PRIMARY KEY,
        EntryDate DATE NOT NULL,
        Type INT NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Description NVARCHAR(200) NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL,
        CONSTRAINT CK_CashEntry_Type CHECK (Type IN (1,2)),
        CONSTRAINT CK_CashEntry_Amount CHECK (Amount > 0)
    );

    CREATE INDEX IX_CashEntry_EntryDate ON dbo.CashEntry(EntryDate);
END
GO

IF OBJECT_ID('dbo.DailyBalance', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DailyBalance
    (
        BalanceDate DATE NOT NULL CONSTRAINT PK_DailyBalance PRIMARY KEY,
        TotalCredits DECIMAL(18,2) NOT NULL CONSTRAINT DF_DailyBalance_TotalCredits DEFAULT 0,
        TotalDebits DECIMAL(18,2) NOT NULL CONSTRAINT DF_DailyBalance_TotalDebits DEFAULT 0,
        Balance DECIMAL(18,2) NOT NULL CONSTRAINT DF_DailyBalance_Balance DEFAULT 0,
        UpdatedAtUtc DATETIME2 NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.OutboxMessage', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OutboxMessage
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_OutboxMessage PRIMARY KEY,
        Type NVARCHAR(150) NOT NULL,
        Payload NVARCHAR(MAX) NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL,
        ProcessedAtUtc DATETIME2 NULL,
        Attempts INT NOT NULL CONSTRAINT DF_OutboxMessage_Attempts DEFAULT 0,
        LastError NVARCHAR(1000) NULL
    );

    CREATE INDEX IX_OutboxMessage_Pending ON dbo.OutboxMessage(ProcessedAtUtc, CreatedAtUtc) INCLUDE (Attempts);
END
GO
