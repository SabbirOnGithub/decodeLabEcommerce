GO
/****** Object:  StoredProcedure [dbo].[ProductLoadFreeDeliveryPaged]    Script Date: 2/12/2019 12:28:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--new stored procedure
CREATE PROCEDURE [dbo].[ProductLoadFreeDeliveryPaged]
(
	@PageIndex				int = 0,
	@PageSize				int = 2147483647,
	@OrderBy				int = 0,
	@PriceMin				decimal = 0,
	@PriceMax				decimal = 2147483647,
	@MinPriceRange			decimal = null OUTPUT,
	@MaxPriceRange			decimal = null OUTPUT,
	@TotalRecords			int = null OUTPUT
)
AS
BEGIN
	DECLARE @sql nvarchar(max)

	SET @PriceMin = ISNULL(@PriceMin, 0) 
	SET @PriceMax = ISNULL(@PriceMax, 2147483647) 

	CREATE TABLE #FREEDELIVERYPRODUCTIDS
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int not null,
		[Price] decimal(18, 4) not null
	)
	SET @sql = 'INSERT #FREEDELIVERYPRODUCTIDS '
	SET @sql = @sql + ' SELECT pr.Id ProductId, pr.Price Price from Product pr where pr.IsFreeShipping = 1 AND pr.Deleted = 0 AND pr.Published = 1 AND pr.Price >= ' + 
		CAST(@PriceMin as nvarchar(max)) + ' AND pr.Price <= ' + CAST(@PriceMax as nvarchar(max)) + ' '
		
	IF(@OrderBy = 5)
		SET @sql = @sql + 'ORDER BY pr.Name ASC'
	IF(@OrderBy = 6)
		SET @sql = @sql + 'ORDER BY pr.Name DESC'
	IF(@OrderBy = 10)
		SET @sql = @sql + 'ORDER BY pr.Price ASC'
	IF(@OrderBy = 11)
		SET @sql = @sql + 'ORDER BY pr.Price Desc'
	IF(@OrderBy = 15)
		SET @sql = @sql + 'ORDER BY pr.CreatedOnUtc'
		
	--PRINT @sql
	EXEC sp_executesql @sql

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1

	SET @MaxPriceRange = (SELECT MAX(pr.Price) FROM Product pr where pr.IsFreeShipping = 1 AND pr.Deleted = 0 AND pr.Published = 1)
	SET @MinPriceRange = (SELECT MIN(pr.Price) FROM Product pr where pr.IsFreeShipping = 1 AND pr.Deleted = 0 AND pr.Published = 1)
	SET @TotalRecords = (SELECT COUNT(*) from Product pr where pr.IsFreeShipping = 1 AND pr.Deleted = 0 AND pr.Published = 1)

	SELECT pr.* FROM Product pr INNER JOIN #FREEDELIVERYPRODUCTIDS r on r.ProductId = pr.Id WHERE r.IndexId > @PageLowerBound AND r.IndexId < @PageUpperBound

	DROP TABLE #FREEDELIVERYPRODUCTIDS
END
