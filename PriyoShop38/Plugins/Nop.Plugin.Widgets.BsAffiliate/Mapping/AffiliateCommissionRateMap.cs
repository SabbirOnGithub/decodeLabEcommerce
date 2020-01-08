using Nop.Plugin.Widgets.BsAffiliate.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Widgets.BsAffiliate.Mapping
{
    public class AffiliateCommissionRateMap : EntityTypeConfiguration<AffiliateCommissionRate>
    {
        public AffiliateCommissionRateMap()
        {
            this.ToTable("BS_AffiliateCommissionRate");
            this.HasKey(x => x.Id);
        }
    }
}
