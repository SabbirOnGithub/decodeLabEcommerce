GO

/****** Object:  Table [dbo].[AlgoliaUpdateItem]    Script Date: 4/16/2019 7:26:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AlgoliaUpdateItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NopEntityId] [int] NOT NULL,
	[NopEntityType] [int] NOT NULL,
	[InstantUpdate] [bit] NOT NULL,
	[DeleteAfterUpdate] [bit] NOT NULL,
	[UpdateDateTimeUtc] [datetime] NULL,
	[DiscountId] [int] NOT NULL,
 CONSTRAINT [PK_AlgoliaUpdateItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO