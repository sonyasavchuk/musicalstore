IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'MusicalStore')
    BEGIN
        CREATE DATABASE [MusicalStore]
    END
GO
USE [MusicalStore]
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Countries] (
    [Id] int NOT NULL IDENTITY,
    [CountryName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Countries] PRIMARY KEY ([Id])
    );
GO

CREATE TABLE [Groups] (
    [Id] int NOT NULL IDENTITY,
    [GroupName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY ([Id])
    );
GO

CREATE TABLE [Materials] (
    [Id] int NOT NULL IDENTITY,
    [MaterialName] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Materials] PRIMARY KEY ([Id])
    );
GO

CREATE TABLE [Manufacturers] (
    [Id] int NOT NULL IDENTITY,
    [ManufacturerName] nvarchar(50) NOT NULL,
    [CountryId] int NOT NULL,
    CONSTRAINT [PK_Manufacturers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Manufacturer_Country] FOREIGN KEY ([CountryId]) REFERENCES [Countries] ([Id])
    );
GO

CREATE TABLE [Types] (
    [Id] int NOT NULL IDENTITY,
    [TypeName] nvarchar(50) NOT NULL,
    [GroupId] int NOT NULL,
    CONSTRAINT [PK_Types] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InstrumentType_InstrumentGroup] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE CASCADE
    );
GO

CREATE TABLE [Instruments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [InstrumentTypeId] int NOT NULL,
    [Price] decimal(18,0) NOT NULL,
    [ManufacturerId] int NOT NULL,
    [ManufactoringCountryId] int NOT NULL,
    CONSTRAINT [PK_Instruments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Instruments_Country] FOREIGN KEY ([ManufactoringCountryId]) REFERENCES [Countries] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Instruments_InstrumentType] FOREIGN KEY ([InstrumentTypeId]) REFERENCES [Types] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Instruments_Manufacturer] FOREIGN KEY ([ManufacturerId]) REFERENCES [Manufacturers] ([Id]) ON DELETE CASCADE
    );
GO

CREATE TABLE [InstrumentMaterial] (
    [Id] int NOT NULL IDENTITY,
    [InstrumentId] int NOT NULL,
    [MaterialId] int NOT NULL,
    CONSTRAINT [PK_InstrumentMaterial] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Instrument/Materials_Instruments] FOREIGN KEY ([InstrumentId]) REFERENCES [Instruments] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Instrument/Materials_Materials] FOREIGN KEY ([MaterialId]) REFERENCES [Materials] ([Id]) ON DELETE CASCADE
    );
GO

CREATE INDEX [IX_InstrumentMaterial_InstrumentId] ON [InstrumentMaterial] ([InstrumentId]);
GO

CREATE INDEX [IX_InstrumentMaterial_MaterialId] ON [InstrumentMaterial] ([MaterialId]);
GO

CREATE INDEX [IX_Instruments_InstrumentTypeId] ON [Instruments] ([InstrumentTypeId]);
GO

CREATE INDEX [IX_Instruments_ManufactoringCountryId] ON [Instruments] ([ManufactoringCountryId]);
GO

CREATE INDEX [IX_Instruments_ManufacturerId] ON [Instruments] ([ManufacturerId]);
GO

CREATE INDEX [IX_Manufacturers_CountryId] ON [Manufacturers] ([CountryId]);
GO

CREATE INDEX [IX_Types_GroupId] ON [Types] ([GroupId]);
GO

CREATE INDEX [IX_Materials_Name] ON [Materials] ([MaterialName]);
GO

COMMIT;
GO
