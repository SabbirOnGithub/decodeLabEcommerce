USE [PriyoShop38]
GO

/****** Object:  Table [dbo].[Discount_AppliedToVendors]    Script Date: 3/3/2019 8:22:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Discount_AppliedToVendors](
	[Discount_Id] [int] NOT NULL,
	[Vendor_id] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Discount_AppliedToVendors]  WITH CHECK ADD  CONSTRAINT [FK_Discount_AppliedToVendors_Discount] FOREIGN KEY([Discount_Id])
REFERENCES [dbo].[Discount] ([Id])
GO

ALTER TABLE [dbo].[Discount_AppliedToVendors] CHECK CONSTRAINT [FK_Discount_AppliedToVendors_Discount]
GO

ALTER TABLE [dbo].[Discount_AppliedToVendors]  WITH CHECK ADD  CONSTRAINT [FK_Discount_AppliedToVendors_Vendor] FOREIGN KEY([Vendor_id])
REFERENCES [dbo].[Vendor] ([Id])
GO

ALTER TABLE [dbo].[Discount_AppliedToVendors] CHECK CONSTRAINT [FK_Discount_AppliedToVendors_Vendor]
GO


