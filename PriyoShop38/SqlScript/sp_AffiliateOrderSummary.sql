USE [PriyoShop38V1]
GO
/****** Object:  StoredProcedure [dbo].[AffiliateOrderSummary]    Script Date: 5/5/2019 3:57:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[AffiliateOrderSummary]
(	
	@AffiliateId				int = 0,
	@CreatedFromUtc				datetime = null,	
	@CreatedToUtc				datetime = null,
	@OrderStatusIds				nvarchar(max) = null,
	@PaymentStatusIds			nvarchar(max) = null,
	@PageIndex					int = 0,
	@PageSize					int = 2147483646,
	@TotalRecords				int = null OUTPUT,
	@TotalCommission			decimal = null OUTPUT,
	@PayableCommission			decimal = null OUTPUT,
	@PaidCommission				decimal = null OUTPUT,
	@UnpaidCommission			decimal = null OUTPUT
)
AS
BEGIN

	SET @OrderStatusIds = isnull(@OrderStatusIds, '')
	SET @PaymentStatusIds = isnull(@PaymentStatusIds, '')

	CREATE TABLE #SearchOrder
	(
		[IndexId] int identity(1,1),
		[OrderId] int NOT NULL,
		[PaymentStatusId] int NOT NULL,
		[OrderStatusId] int NOT NULL,
		[AffiliateCommission] decimal NOT NULL,
		[IsCommissionPaid] bit NOT NULL
	)

	DECLARE @sql nvarchar(max)

	SET @sql = '
		INSERT INTO #SearchOrder ([OrderId], [PaymentStatusId], [OrderStatusId], [AffiliateCommission], [IsCommissionPaid])
		SELECT o.[Id], o.[PaymentStatusId], o.[OrderStatusId], o.[AffiliateCommission], o.[IsCommissionPaid]
		FROM [Order] o with (NOLOCK)
		WHERE o.Deleted = 0 '

	if(@CreatedFromUtc is not null)
		set @sql = @sql + 'AND o.CreatedOnUtc >= ''' + CAST(@CreatedFromUtc AS nvarchar(max)) + ''' '
		
	if(@CreatedToUtc is not null)
		set @sql = @sql + 'AND o.CreatedOnUtc <= ''' + CAST(@CreatedToUtc AS nvarchar(max)) + ''' '
	
	if(@AffiliateId > 0)
		set @sql = @sql + 'AND o.AffiliateId = ' + CAST(@AffiliateId AS nvarchar(max)) + ' '
	
	set @sql = @sql + ' order by o.CreatedOnUtc desc'

	EXEC sp_executesql @sql

	set @TotalRecords = @@ROWCOUNT

	Select @TotalCommission = SUM([AffiliateCommission]) from #SearchOrder

	Select @PayableCommission = SUM([AffiliateCommission]) from #SearchOrder WHERE PaymentStatusId = 30

	Select @PaidCommission = SUM([AffiliateCommission]) from #SearchOrder WHERE PaymentStatusId = 30 AND IsCommissionPaid = 1

	Select @UnpaidCommission = SUM([AffiliateCommission]) from #SearchOrder WHERE PaymentStatusId = 30 AND IsCommissionPaid = 0

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	DECLARE @RowsToReturn int
	SET @RowsToReturn = @PageSize * (@PageIndex + 1)	
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1

	SELECT TOP (@RowsToReturn)
		o.*
	FROM
		#SearchOrder [oi]
		INNER JOIN [Order] o with (NOLOCK) on o.Id = [oi].[OrderId]
	WHERE
		[oi].IndexId > @PageLowerBound AND 
		[oi].IndexId < @PageUpperBound
	ORDER BY
		[oi].IndexId

	DROP TABLE #SearchOrder
END





