ALTER TABLE Product ADD VendorInActiveOrDeleted Bit NOT NULL DEFAULT ([dbo].[nop_getVendorInActiveOrDeleted]([VendorId]))

ALTER TABLE Product ADD RecentlyUpdated Bit NOT NULL DEFAULT 0

ALTER TABLE Product ADD LastUpdatedBy int NOT NULL DEFAULT 0

ALTER TABLE Product ADD EkshopOnly int NOT NULL DEFAULT 0