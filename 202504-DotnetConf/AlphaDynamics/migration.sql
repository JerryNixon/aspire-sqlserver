IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Crews] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Crews] PRIMARY KEY ([Id])
);

CREATE TABLE [Equipment] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Equipment] PRIMARY KEY ([Id])
);

CREATE TABLE [Operations] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Operations] PRIMARY KEY ([Id])
);

CREATE TABLE [Transactions] (
    [Id] int NOT NULL IDENTITY,
    [Date] datetime2 NOT NULL,
    [CrewId] int NOT NULL,
    [EquipmentId] int NOT NULL,
    [OperationId] int NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transactions_Crews_CrewId] FOREIGN KEY ([CrewId]) REFERENCES [Crews] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Transactions_Equipment_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [Equipment] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Transactions_Operations_OperationId] FOREIGN KEY ([OperationId]) REFERENCES [Operations] ([Id]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Operations]'))
    SET IDENTITY_INSERT [Operations] ON;
INSERT INTO [Operations] ([Id], [Name])
VALUES (1, N'CheckOut'),
(2, N'CheckIn');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Operations]'))
    SET IDENTITY_INSERT [Operations] OFF;

CREATE INDEX [IX_Transactions_CrewId] ON [Transactions] ([CrewId]);

CREATE INDEX [IX_Transactions_EquipmentId] ON [Transactions] ([EquipmentId]);

CREATE INDEX [IX_Transactions_OperationId] ON [Transactions] ([OperationId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250418234942_InitialCreate', N'9.0.4');

COMMIT;
GO

