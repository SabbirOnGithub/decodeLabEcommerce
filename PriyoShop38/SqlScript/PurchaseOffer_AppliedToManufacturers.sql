USE [PriyoShop38]
GO

/****** Object:  Table [dbo].[PurchaseOffer_AppliedToManufacturers]    Script Date: 5/26/2019 3:36:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOffer_AppliedToManufacturers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManufacturerId] [int] NOT NULL,
	[PurchaseOfferId] [int] NOT NULL,
 CONSTRAINT [PK_PurchaseOfferManufacturer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToManufacturers]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferManufacturer_Manufacturer] FOREIGN KEY([ManufacturerId])
REFERENCES [dbo].[Manufacturer] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToManufacturers] CHECK CONSTRAINT [FK_PurchaseOfferManufacturer_Manufacturer]
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToManufacturers]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferManufacturer_PurchaseOffer] FOREIGN KEY([PurchaseOfferId])
REFERENCES [dbo].[PurchaseOffer] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToManufacturers] CHECK CONSTRAINT [FK_PurchaseOfferManufacturer_PurchaseOffer]
GO


