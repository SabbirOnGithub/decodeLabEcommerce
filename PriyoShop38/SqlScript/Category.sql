ALTER TABLE Category ADD IsFreeShipping Bit NOT NULL DEFAULT 0  

ALTER TABLE Category ADD VoucherStartsOn datetime NULL 

ALTER TABLE Category ADD AffiliateCommissionRate decimal(18, 4) NOT NULL DEFAULT 0  

ALTER TABLE Category ADD RecentlyUpdated Bit NOT NULL DEFAULT 0

ALTER TABLE Category ADD LastUpdatedBy int NOT NULL DEFAULT 0