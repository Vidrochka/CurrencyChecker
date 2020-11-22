--Предпологается что Currency сгенерирован EF или добавлен руками
USE [Currency]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Fantom
-- Create date: *today*
-- Description:	Select curs value by date and name
-- =============================================
CREATE PROCEDURE [dbo].[SelectExchangeRateValue]
	@Date date,
	@CharCode varchar(max)
AS
BEGIN

	SET NOCOUNT ON;

	SELECT [er].[Nominal], [er].[Value]
	FROM [dbo].[ExchangeRates] AS er
	WHERE [er].[ExchangeRateDate] = @Date 
	AND [er].[CharCode] = @CharCode

END
GO
