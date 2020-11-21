--Можно не использовать т.к. автоматически генерится EF, но не критично, EF6 не будет пересоздавать
--Предпологается что Currency сгенерирован EF
USE [Currency]
GO

CREATE TABLE [ExchangeRates] 
(
    [ExchangeRateDate] datetime2 NOT NULL,
    [CharCode] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Nominal] int NOT NULL,
    [Value] decimal(18,4) NOT NULL,
    CONSTRAINT [PK_ExchangeRates] PRIMARY KEY ([ExchangeRateDate], [CharCode])
)

GO

CREATE INDEX [IX_ExchangeRates_CharCode_ExchangeRateDate] ON [ExchangeRates] ([CharCode], [ExchangeRateDate])
GO