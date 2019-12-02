using Nop.Core.Domain.Discounts;
using System;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Nop.Data.Mapping.Discounts
{
    public partial class PurchaseOfferUsageHistoryMap : NopEntityTypeConfiguration<PurchaseOfferUsageHistory>
    {
        public PurchaseOfferUsageHistoryMap()
        {
            this.ToTable("PurchaseOfferUsageHistory");
            this.HasKey(duh => duh.Id);

            this.HasRequired(duh => duh.PurchaseOffer)
                .WithMany()
                .HasForeignKey(duh => duh.PurchaseOfferId);

            this.HasRequired(duh => duh.Order)
                .WithOptional(o => o.PurchaseOfferUsageHistory)
                .Map(configurationAction: new Action<ForeignKeyAssociationMappingConfiguration>(x => x.MapKey("OrderId")));

            this.HasRequired(duh => duh.GiftProduct)
                .WithMany()
                .HasForeignKey(duh => duh.GiftProductId);
        }
    }
}
