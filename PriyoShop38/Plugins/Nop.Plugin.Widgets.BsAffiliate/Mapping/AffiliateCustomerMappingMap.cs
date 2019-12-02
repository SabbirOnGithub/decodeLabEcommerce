using Nop.Plugin.Widgets.BsAffiliate.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Widgets.BsAffiliate.Mapping
{
    public class AffiliateCustomerMappingMap : EntityTypeConfiguration<AffiliateCustomerMapping>
    {
        public AffiliateCustomerMappingMap()
        {
            this.ToTable("BS_AffiliateCustomer_Mapping");
            this.HasKey(x => x.Id);
        }
    }
}
