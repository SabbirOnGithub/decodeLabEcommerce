USE [PriyoShop38]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProductHistory]  WITH CHECK ADD  CONSTRAINT [ProductHistory_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProductHistory] CHECK CONSTRAINT [ProductHistory_Customer]
GO

ALTER TABLE [dbo].[ProductHistory]  WITH CHECK ADD  CONSTRAINT [ProductHistory_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProductHistory] CHECK CONSTRAINT [ProductHistory_Product]
GO