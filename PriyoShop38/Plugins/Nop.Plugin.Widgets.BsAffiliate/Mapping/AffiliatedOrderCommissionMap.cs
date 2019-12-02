using Nop.Plugin.Widgets.BsAffiliate.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Widgets.BsAffiliate.Mapping
{
    public class AffiliatedOrderCommissionMap : EntityTypeConfiguration<AffiliatedOrderCommission>
    {
        public AffiliatedOrderCommissionMap()
        {
            this.ToTable("BS_AffiliatedOrderCommission");
            this.HasKey(x => x.Id);
        }
    }
}
