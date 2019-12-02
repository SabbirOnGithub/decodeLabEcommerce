USE [PriyoShop38]
GO

/****** Object:  Table [dbo].[PurchaseOffer]    Script Date: 5/26/2019 3:36:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PurchaseOffer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MinimumCartAmount] [decimal](18, 4) NOT NULL,
	[ForAllProducts] [bit] NOT NULL,
	[GiftProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[StartDateUtc] [datetime] NULL,
	[EndDateUtc] [datetime] NULL,
 CONSTRAINT [PK_PurchaseOffer_2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


