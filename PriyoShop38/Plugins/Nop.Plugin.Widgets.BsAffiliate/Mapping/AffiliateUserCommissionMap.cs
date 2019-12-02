using Nop.Plugin.Widgets.BsAffiliate.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Widgets.BsAffiliate.Mapping
{
    public partial class AffiliateUserCommissionMap : EntityTypeConfiguration<AffiliateUserCommission>
    {
        public AffiliateUserCommissionMap()
        {
            this.ToTable("BS_AffiliateUserCommission");
            this.HasKey(x => x.Id);
        }
    }
}
