using Nop.Plugin.Widgets.EkShopA2I.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Widgets.EkShopA2I.Data
{
    public class EsOrderMap : EntityTypeConfiguration<EsOrder>
    {
        public EsOrderMap()
        {
            this.ToTable("ES_Order");
            this.HasKey(x => x.Id);
        }
    }
}
