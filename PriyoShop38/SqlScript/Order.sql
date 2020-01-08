ALTER TABLE [Order] ADD UserAgentTypeId int NOT NULL DEFAULT 0  

UPDATE  [Order] 
   SET [Order].UserAgentTypeId = 
   CASE 
      WHEN IsNull(GenericAttribute.[Value], '') = 'Mobile' THEN 20
      WHEN IsNull(GenericAttribute.[Value], '') = 'Web' THEN 10
      ELSE 5
	END
   FROM [Order]  INNER JOIN  GenericAttribute ON [Order].id = GenericAttribute.EntityId 
   AND GenericAttribute.KeyGroup = 'Order' AND GenericAttribute.[Key] = 'UserAgent'
   
   
 ALTER TABLE [Order] ADD AffiliateCommission decimal(18, 4) NOT NULL DEFAULT 0  
   
 ALTER TABLE [Order] ADD IsCommissionPaid bit NOT NULL DEFAULT 0  
   
 ALTER TABLE [Order] ADD CommissionPaidOn datetime NULL
 

UPDATE  [Order] 
   SET [Order].AffiliateCommission =  ISNULL([BS_AffiliatedOrderCommission].TotalCommission, 0),
   [Order].IsCommissionPaid = 
   CASE 
      WHEN [BS_AffiliatedOrderCommission].PaymentStatus = 20 THEN 1
      ELSE 0
	END,
	[order].CommissionPaidOn = [BS_AffiliatedOrderCommission].MarkedAsPaidOn
   FROM [Order]  INNER JOIN  [BS_AffiliatedOrderCommission] ON [Order].id = [BS_AffiliatedOrderCommission].OrderId 