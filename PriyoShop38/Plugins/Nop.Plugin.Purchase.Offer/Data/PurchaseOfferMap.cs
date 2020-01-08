using Nop.Plugin.Purchase.Offer.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Purchase.Offer.Data
{
    public class PurchaseOfferMap : EntityTypeConfiguration<PurchaseOffer>
    {
        public PurchaseOfferMap()
        {
            ToTable("PurchaseOffer");
        }
    }
}
