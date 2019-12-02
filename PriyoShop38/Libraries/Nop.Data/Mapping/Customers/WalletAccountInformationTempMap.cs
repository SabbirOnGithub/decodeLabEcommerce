using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public class WalletAccountInformationTempMap : NopEntityTypeConfiguration<WalletAccountInformationTemp>
    {
        public WalletAccountInformationTempMap()
        {
            ToTable("WalletAccountInformationTemp", "Customer");
            HasKey(e => e.Id);

            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}