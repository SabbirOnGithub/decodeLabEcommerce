
ALTER TABLE BS_Slider ADD SliderDomainTypeId int NOT NULL DEFAULT 0

ALTER TABLE BS_Slider ADD DomainId int NOT NULL DEFAULT 0

ALTER TABLE BS_Slider ADD DisplayOrder int NOT NULL DEFAULT 0

ALTER TABLE BS_Slider DROP COLUMN IsProduct;

ALTER TABLE BS_Slider DROP COLUMN ProdOrCatId;