USE [PriyoShop38]
GO

/****** Object:  Table [dbo].[PurchaseOffer_AppliedToProducts]    Script Date: 5/26/2019 3:36:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOffer_AppliedToProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[PurchaseOfferId] [int] NOT NULL,
 CONSTRAINT [PK_PurchaseOfferProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToProducts]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferProduct_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToProducts] CHECK CONSTRAINT [FK_PurchaseOfferProduct_Product]
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToProducts]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferProduct_PurchaseOffer] FOREIGN KEY([PurchaseOfferId])
REFERENCES [dbo].[PurchaseOffer] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToProducts] CHECK CONSTRAINT [FK_PurchaseOfferProduct_PurchaseOffer]
GO


