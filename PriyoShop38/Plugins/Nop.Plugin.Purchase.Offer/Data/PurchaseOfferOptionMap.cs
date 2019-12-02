using Nop.Plugin.Purchase.Offer.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Purchase.Offer.Data
{
    public class PurchaseOfferOptionMap : EntityTypeConfiguration<PurchaseOfferOption>
    {
        public PurchaseOfferOptionMap()
        {
            ToTable("PurchaseOffer_Option");
        }
    }
}
