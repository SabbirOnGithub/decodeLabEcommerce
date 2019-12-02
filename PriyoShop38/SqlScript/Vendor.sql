ALTER TABLE Vendor ADD IsFreeShipping Bit NOT NULL DEFAULT 0 

ALTER TABLE Vendor ADD AffiliateCommissionRate decimal(18, 4) NOT NULL DEFAULT 0  

UPDATE  [Vendor] 
   SET [Vendor].[AffiliateCommissionRate] = 
   [BS_AffiliateCommissionRate].CommissionRate
   FROM [Vendor]  INNER JOIN  [BS_AffiliateCommissionRate] ON [Vendor].id = [BS_AffiliateCommissionRate].EntityId AND [BS_AffiliateCommissionRate].EntityType = 30 

ALTER TABLE [Vendor] ADD RecentlyUpdated Bit NOT NULL DEFAULT 0

ALTER TABLE [Vendor] ADD LastUpdatedBy int NOT NULL DEFAULT 0

ALTER TABLE [Vendor] ADD UpdatedOnUtc DateTime NOT NULL DEFAULT GETDATE()