GO
/****** Object:  UserDefinedFunction [dbo].[nop_getnotnullnotempty]    Script Date: 4/21/2019 2:48:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[nop_getVendorInActiveOrDeleted]
(
    @vendorId int null = 0
)
RETURNS bit
AS
BEGIN
	IF ((SELECT COUNT(Id) FROM [Vendor] WHERE Id = @vendorId AND (Active = 0 OR Deleted = 1)) > 0)
		RETURN 1;
	ELSE
		RETURN 0;

	RETURN 0;
END
