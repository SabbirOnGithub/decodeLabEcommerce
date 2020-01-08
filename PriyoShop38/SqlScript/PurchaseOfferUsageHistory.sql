USE [PriyoShop38]
GO

/****** Object:  Table [dbo].[PurchaseOfferUsageHistory]    Script Date: 5/26/2019 3:37:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOfferUsageHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOfferId] [int] NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[GiftProductId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
 CONSTRAINT [PK_PurchaseOfferUsageHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PurchaseOfferUsageHistory] ADD  CONSTRAINT [DF_PurchaseOfferUsageHistory_Quantity]  DEFAULT ((0)) FOR [Quantity]
GO

ALTER TABLE [dbo].[PurchaseOfferUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferUsageHistory_Product] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOfferUsageHistory] CHECK CONSTRAINT [FK_PurchaseOfferUsageHistory_Product]
GO

ALTER TABLE [dbo].[PurchaseOfferUsageHistory]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOfferUsageHistory_PurchaseOffer] FOREIGN KEY([PurchaseOfferId])
REFERENCES [dbo].[PurchaseOffer] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PurchaseOfferUsageHistory] CHECK CONSTRAINT [FK_PurchaseOfferUsageHistory_PurchaseOffer]
GO


