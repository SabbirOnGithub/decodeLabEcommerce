using Nop.Plugin.Widgets.EkShopA2I.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Widgets.EkShopA2I.Data
{
    public class EsUdcCommissionRateMap : EntityTypeConfiguration<EsUdcCommissionRate>
    {
        public EsUdcCommissionRateMap()
        {
            this.ToTable("ES_UdcCommissionRate");
            this.HasKey(x => x.Id);
        }
    }
}
