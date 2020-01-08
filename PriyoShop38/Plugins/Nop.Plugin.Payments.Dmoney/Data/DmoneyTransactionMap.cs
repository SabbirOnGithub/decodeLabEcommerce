using Nop.Data.Mapping;
using Nop.Plugin.Payments.Dmoney.Domains;

namespace Nop.Plugin.Payments.Dmoney.Data
{
    public class DmoneyTransactionMap : NopEntityTypeConfiguration<DmoneyTransaction>
    {
        public DmoneyTransactionMap()
        {
            this.ToTable("DmoneyTransaction");
            this.HasKey(x => x.Id);
        }
    }
}