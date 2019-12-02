ALTER TABLE Affiliate ADD CommissionRate decimal(18, 4) NOT NULL DEFAULT 0 

ALTER TABLE Affiliate ADD CustomerId int NOT NULL DEFAULT 0 

ALTER TABLE Affiliate ADD BKashNumber nvarchar(20) NULL

ALTER TABLE Affiliate ADD AffiliateTypeId int NULL 

ALTER TABLE [Affiliate] DROP CONSTRAINT [FK_Affiliate_AffiliateType]

UPDATE  [Affiliate] 
   SET [Affiliate].[CustomerId] = 
   [BS_AffiliateCustomer_Mapping].CustomerId,
   [Affiliate].AffiliateTypeId = 
   CASE 
      WHEN [BS_AffiliateCustomer_Mapping].AffiliateTypeId is null THEN null
      WHEN [BS_AffiliateCustomer_Mapping].AffiliateTypeId  = 0 THEN null
      ELSE [BS_AffiliateCustomer_Mapping].AffiliateTypeId
	END
   FROM [Affiliate]  INNER JOIN  BS_AffiliateCustomer_Mapping ON [Affiliate].id = BS_AffiliateCustomer_Mapping.AffiliateId


ALTER TABLE [dbo].[Affiliate]  WITH CHECK ADD  CONSTRAINT [FK_Affiliate_AffiliateType] FOREIGN KEY([AffiliateTypeId])
REFERENCES [dbo].[AffiliateType] ([Id])