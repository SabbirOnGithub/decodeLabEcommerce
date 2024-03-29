USE [PriyoShop38]
GO
/****** Object:  StoredProcedure [dbo].[CustomerFilterPaged]    Script Date: 3/3/2019 2:10:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[CustomerFilterPaged]
(
	@CreatedFromUtc				datetime = null,	
	@CreatedToUtc				datetime = null,	
	@AffiliateId				int = 0,
	@VendorId					int = 0,
	@CustomerRoleIds			nvarchar(MAX) = null,
	@Email						nvarchar(MAX) = null,
	@Username					nvarchar(MAX) = null,
	@FirstName					nvarchar(MAX) = null,
	@LastName					nvarchar(MAX) = null,
	@DayOfBirth					int = 0,
	@MonthOfBirth				int = 0,
	@Company					nvarchar(MAX) = null,
	@Phone						nvarchar(MAX) = null,
	@ZipCode					nvarchar(MAX) = null,
	@IpAddress					nvarchar(MAX) = null,
	@LoadOnlyWithShoppingCart	bit = 0,
	@ShoppingCartType			int = null,
	@PageIndex					int = 0, 
	@PageSize					int = 2147483644,
	@TotalRecords				int = null OUTPUT
)
AS
BEGIN

	DECLARE @DateOfBirthStr nvarchar(max)
	DECLARE @DayOfBirthStr nvarchar(max)
	DECLARE @MonthOfBirthStr nvarchar(max)

	SET @CustomerRoleIds = isnull(@CustomerRoleIds, '')
	SET @Email = isnull(@Email, '')
	SET @Username = isnull(@Username, '')
	SET @FirstName = isnull(@FirstName, '')
	SET @LastName = isnull(@LastName, '')
	SET @Company = isnull(@Company, '')
	SET @Phone = isnull(@Phone, '')
	SET @ZipCode = isnull(@ZipCode, '')
	SET @IpAddress = isnull(@IpAddress, '')
	
	if(@DayOfBirth > 0)
		BEGIN
			if(@DayOfBirth < 10)
				SET @DayOfBirthStr = '0' + CAST(@DayOfBirth AS nvarchar(max))
			ELSE
				SET @DayOfBirthStr = CAST(@DayOfBirth AS nvarchar(max))
		END
	if(@MonthOfBirth > 0)
		BEGIN
			if(@MonthOfBirth < 10)
				SET @MonthOfBirthStr = '0' + CAST(@MonthOfBirth AS nvarchar(max))
			ELSE
				SET @MonthOfBirthStr = CAST(@MonthOfBirth AS nvarchar(max))
		END

	SET @DayOfBirthStr = isnull(@DayOfBirthStr, '')
	SET @MonthOfBirthStr = isnull(@MonthOfBirthStr, '')
	SET @DateOfBirthStr = isnull(@DateOfBirthStr, '')

	if(@DayOfBirthStr != '' AND @MonthOfBirthStr != '')
		Set @DateOfBirthStr = @DateOfBirthStr + '-' + @MonthOfBirthStr

	CREATE TABLE #SearchCustomer
	(
		[Index] int identity(1,1),
		[CustomerId] int NOT NULL
	)

	DECLARE @sql nvarchar(max)

	SET @sql = '
		INSERT INTO #SearchCustomer ([CustomerId])
		SELECT c.Id
		FROM Customer c with (NOLOCK)
		WHERE c.Deleted = 0 '

	if(@CreatedFromUtc is not null)
		set @sql = @sql + 'AND c.CreatedOnUtc >= ''' + CAST(@CreatedFromUtc AS nvarchar(max)) + ''' '
		
	if(@CreatedToUtc is not null)
		set @sql = @sql + 'AND c.CreatedOnUtc <= ''' + CAST(@CreatedToUtc AS nvarchar(max)) + ''' '
	
	if(@AffiliateId > 0)
		set @sql = @sql + 'AND c.AffiliateId = ' + CAST(@AffiliateId AS nvarchar(max)) + ' '
	
	if(@VendorId > 0)
		set @sql = @sql + 'AND c.VendorId = ' + CAST(@VendorId AS nvarchar(max)) + ' '
	
	if(@CustomerRoleIds != '')
		set @sql = @sql + 'AND c.Id IN (SELECT ccm.Customer_Id FROM Customer_CustomerRole_Mapping ccm WHERE ccm.CustomerRole_Id in (' + @CustomerRoleIds + ')) '
	
	if(@Email != '')
		set @sql = @sql + 'AND c.Email LIKE ''%' + @Email + '%'' '
	
	if(@Username != '')
		set @sql = @sql + 'AND c.Username LIKE ''%' + @Username + '%'' '
	
	if(@FirstName != '')
		set @sql = @sql + 'AND c.Id IN (SELECT ga.EntityId FROM GenericAttribute ga WHERE ga.KeyGroup = ''Customer'' AND ga.[Key] = ''FirstName'' AND ga.[Value] LIKE ''%' + @FirstName + '%'') '
	
	if(@LastName != '')
		set @sql = @sql + 'AND c.Id IN (SELECT ga.EntityId FROM GenericAttribute ga WHERE ga.KeyGroup = ''Customer'' AND ga.[Key] = ''LastName'' AND ga.[Value] LIKE ''%' + @LastName + '%'') '
	
	if(@DayOfBirth > 0)
		set @sql = @sql + 'AND c.Id IN (SELECT ga.EntityId FROM GenericAttribute ga WHERE ga.KeyGroup = ''Customer'' AND ga.[Key] = ''DateOfBirth'' AND SUBSTRING( ga.[Value], 6, 2) = ''' + @DayOfBirthStr + ''') '
	
	if(@MonthOfBirthStr > 0)
		set @sql = @sql + 'AND c.Id IN (SELECT ga.EntityId FROM GenericAttribute ga WHERE ga.KeyGroup = ''Customer'' AND ga.[Key] = ''DateOfBirth'' AND SUBSTRING( ga.[Value], 9, 2) = ''' + @MonthOfBirthStr + ''') '
	
	if(@Company != '')
		set @sql = @sql + 'AND c.Id IN (SELECT ga.EntityId FROM GenericAttribute ga WHERE ga.KeyGroup = ''Customer'' AND ga.[Key] = ''Company'' AND ga.[Value] LIKE ''%' + @Company + '%'') '
	
	if(@Phone != '')
		set @sql = @sql + 'AND c.Id IN (SELECT ga.EntityId FROM GenericAttribute ga WHERE ga.KeyGroup = ''Customer'' AND ga.[Key] = ''Phone'' AND ga.[Value] LIKE ''%' + @Phone + '%'') '
	
	if(@ZipCode != '')
		set @sql = @sql + 'AND c.Id IN (SELECT ga.EntityId FROM GenericAttribute ga WHERE ga.KeyGroup = ''Customer'' AND ga.[Key] = ''ZipPostalCode'' AND ga.[Value] LIKE ''%' + @ZipCode + '%'') '
	
	if(@IpAddress != '')
		set @sql = @sql + 'AND c.LastIpAddress LIKE ''%' + @IpAddress + '%'' '

	if(@LoadOnlyWithShoppingCart = 1)
		BEGIN
			if (@ShoppingCartType is not null)
				set @sql = @sql + 'AND c.Id in (Select DISTINCT sci.CustomerId from ShoppingCartItem sci WHERE sci.ShoppingCartTypeId = ' + CAST(@ShoppingCartType as nvarchar(max)) + ') '
			ELSE
				set @sql = @sql + 'AND c.Id in (Select DISTINCT sci.CustomerId from ShoppingCartItem sci) '
		END

	SET @sql = @sql + 'Order by c.CreatedOnUtc'

	EXEC sp_executesql @sql

	set @TotalRecords = @@ROWCOUNT

	--paging
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	SET @PageLowerBound = @PageSize * @PageIndex
	SET @PageUpperBound = @PageLowerBound + @PageSize + 1

	SELECT cs.* FROM Customer cs inner join #SearchCustomer sc  on sc.CustomerId = cs.Id WHERE sc.[Index]> @PageLowerBound AND sc.[Index] < @PageUpperBound

END





