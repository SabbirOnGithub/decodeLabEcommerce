
CREATE PROCEDURE [dbo].[SmartGroup_BsNotification]  --'', '', 'CustomerRole.Active  =  ''True''', '[Key] = ''City'' And Value  Like  ''%Darien%'' And CustomerRole.Active  =  ''True'''
(
	@CustomerWhere		nvarchar(500) = '',	--a list of category IDs (comma-separated list). e.g. 1,2,3
	@NewsLetterWhere		nvarchar(500) = '',
	@CustomerRoleWhere		nvarchar(500) = '',
	@OthersWhere		nvarchar(500) = ''
)
AS
BEGIN
	
	/* Products that filtered by keywords */
	CREATE TABLE #SmartGroup_Email_List
	(
	    [FirstName] nvarchar(300) NULL,
	    [LastName] nvarchar(300) NULL,
		[UserName] nvarchar(300) NULL,
		[Email] nvarchar(300) NULL,
		[CustomerId] integer not null default(0),
		[CreatedOnUtc] DateTime NULL
	)
	
	DECLARE
		@sql nvarchar(1000)
		
	IF((@CustomerWhere != '') And (@CustomerRoleWhere = '') And (@OthersWhere = ''))
	Begin
		SET @sql = '
			INSERT INTO #SmartGroup_Email_List ([UserName],[Email],[CreatedOnUtc],[CustomerId])
			SELECT Username, Email, CreatedOnUtc, Id
			FROM Customer WHERE '
		Set	@sql = @sql+@CustomerWhere
		
		EXEC sp_executesql @sql
		
		
	End
	IF(@NewsLetterWhere != '')
	Begin
		SET @sql = '
			INSERT INTO #SmartGroup_Email_List ([Email],[CreatedOnUtc])
			SELECT Email,CreatedOnUtc
			FROM NewsLetterSubscription WHERE '
		Set	@sql = @sql+@NewsLetterWhere
		
		EXEC sp_executesql @sql
		
		
	End
	IF((@CustomerRoleWhere != '' ) And (@OthersWhere = ''))
	Begin
		SET @sql = '
			INSERT INTO #SmartGroup_Email_List ([UserName],[Email],[CreatedOnUtc],[CustomerId])
			SELECT Username, Email, Customer.CreatedOnUtc, Customer.Id
			FROM Customer INNER JOIN
            Customer_CustomerRole_Mapping ON Customer.Id = Customer_CustomerRole_Mapping.Customer_Id INNER JOIN
            CustomerRole ON Customer_CustomerRole_Mapping.CustomerRole_Id = CustomerRole.Id
			INNER JOIN Bs_WebApi_Device ON Customer.Id = Bs_WebApi_Device.CustomerId WHERE '
		Set	@sql = @sql+@CustomerRoleWhere
		
		EXEC sp_executesql @sql
		
		
	End
	IF((@OthersWhere != '') And (@CustomerRoleWhere = ''))
	Begin
		SET @sql = '
			INSERT INTO #SmartGroup_Email_List ([UserName],[Email],[CreatedOnUtc],[CustomerId])
			SELECT UserName, Email, CreatedOnUtc, Customer.Id
			FROM Customer INNER JOIN
				GenericAttribute ON Customer.Id = GenericAttribute.EntityId
			WHERE (GenericAttribute.KeyGroup = ''Customer'') AND '  
			
		Set	@sql = @sql+@OthersWhere
		
		EXEC sp_executesql @sql
		
	End
	IF((@OthersWhere != '') And (@CustomerRoleWhere != ''))
	Begin
		SET @sql = '
			INSERT INTO #SmartGroup_Email_List ([UserName],[Email],[CreatedOnUtc],[CustomerId])
			Select Customer.Username, Customer.Email, Customer.CreatedOnUtc, Customer.Id
			FROM Customer INNER JOIN
					Customer_CustomerRole_Mapping ON Customer.Id = Customer_CustomerRole_Mapping.Customer_Id INNER JOIN
					CustomerRole ON Customer_CustomerRole_Mapping.CustomerRole_Id = CustomerRole.Id INNER JOIN
					GenericAttribute ON Customer.Id = GenericAttribute.EntityId
			WHERE (GenericAttribute.KeyGroup = ''Customer'') And '  
			
		Set	@sql = @sql+@OthersWhere
		
		EXEC sp_executesql @sql
		
	End
	SET @sql = 'SELECT DISTINCT CustomerId, FirstName,LastName, Email, UserName,  CreatedOnUtc FROM #SmartGroup_Email_List'
			
	EXEC sp_executesql @sql
	RETURN @sql
	
	DROP TABLE #SmartGroup_Email_List
End