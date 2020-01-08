USE [PriyoShop38]
GO

/****** Object:  Table [dbo].[PurchaseOffer_AppliedToCategories]    Script Date: 5/26/2019 3:36:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOffer_AppliedToCategories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[PurchaseOfferId] [int] NOT NULL,
 CONSTRAINT [PK_PurchaseOfferCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToCategories]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferCategory_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToCategories] CHECK CONSTRAINT [FK_PurchaseOfferCategory_Category]
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToCategories]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferCategory_PurchaseOffer] FOREIGN KEY([PurchaseOfferId])
REFERENCES [dbo].[PurchaseOffer] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOffer_AppliedToCategories] CHECK CONSTRAINT [FK_PurchaseOfferCategory_PurchaseOffer]
GO


