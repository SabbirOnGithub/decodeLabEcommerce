GO
/****** Object:  StoredProcedure [dbo].[ProductLoadAlgoliaUpdatePaged]    Script Date: 4/16/2019 7:26:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[ProductLoadAlgoliaUpdatePaged]
(
	@CategoryIds		nvarchar(MAX) = null,	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@VendorIds			nvarchar(MAX) = null,	--a list of vendor IDs (comma-separated list). e.g. 1,2,3
	@ManufacturerIds	nvarchar(MAX) = null,	--a list of manufacturer IDs (comma-separated list). e.g. 1,2,3
	@ProductIds			nvarchar(MAX) = null,	--a list of product IDs (comma-separated list). e.g. 1,2,3
	@PageIndex			int = 0, 
	@PageSize			int = 2147483644,
	@TotalRecords		int = null OUTPUT
)
AS
BEGIN
	DECLARE @sql nvarchar(max)
	
	--filter by keywords
	SET @CategoryIds = isnull(@CategoryIds, '')
	SET @VendorIds = isnull(@VendorIds, '')
	SET @ManufacturerIds = isnull(@ManufacturerIds, '')
	SET @ProductIds = isnull(@ProductIds, '')
	
	CREATE TABLE #ALGOLIAUPDATEPRODUCTIDS
	(
		[IndexId] int IDENTITY (1, 1) NOT NULL,
		[ProductId] int not null
	)
	SET @sql = 'INSERT #ALGOLIAUPDATEPRODUCTIDS '
	SET @sql = @sql + ' SELECT pr.Id ProductId from Product pr '
	
	if(@CategoryIds != '')
		SET @sql = @sql + ' LEFT JOIN Product_Category_Mapping pcm on pcm.ProductId = pr.Id'
	
	if(@ManufacturerIds != '')
		SET @sql = @sql + ' LEFT JOIN Product_Manufacturer_Mapping pmm on pmm.ProductId = pr.Id'

	SET @sql = @sql + ' where pr.Deleted = 0 AND pr.Published = 1 '
		
	
	if(@CategoryIds != '')
		SET @sql = @sql + ' AND pcm.CategoryId in (' + @CategoryIds + ') ' 
	
	if(@VendorIds != '')
		SET @sql = @sql + ' AND pr.VendorId in (' + @VendorIds + ') '
	
	if(@ManufacturerIds != '')
		SET @sql = @sql + ' AND pmm.ManufacturerId in (' + @ManufacturerIds + ') '
	
	if(@ProductIds != '')
		SET @sql = @sql + ' AND pr.Id in (' + @ProductIds + ') '

	--PRINT @sql
	EXEC sp_executesql @sql

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1

	SET @TotalRecords = @@rowcount

	SELECT pr.* FROM Product pr INNER JOIN #ALGOLIAUPDATEPRODUCTIDS r on r.ProductId = pr.Id WHERE r.IndexId > @PageLowerBound AND r.IndexId < @PageUpperBound

	DROP TABLE #ALGOLIAUPDATEPRODUCTIDS
END
