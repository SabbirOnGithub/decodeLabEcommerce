USE [PriyoShop38]
GO

/****** Object:  Table [dbo].[PurchaseOffer_AppliedToVendors]    Script Date: 5/26/2019 3:37:04 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOffer_AppliedToVendors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[VendorId] [int] NOT NULL,
	[PurchaseOfferId] [int] NOT NULL,
 CONSTRAINT [PK_PurchaseOfferVendor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToVendors]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferVendor_PurchaseOffer] FOREIGN KEY([PurchaseOfferId])
REFERENCES [dbo].[PurchaseOffer] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToVendors] CHECK CONSTRAINT [FK_PurchaseOfferVendor_PurchaseOffer]
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToVendors]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferVendor_Vendor] FOREIGN KEY([VendorId])
REFERENCES [dbo].[Vendor] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToVendors] CHECK CONSTRAINT [FK_PurchaseOfferVendor_Vendor]
GO


