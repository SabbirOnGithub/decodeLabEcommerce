using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public class WalletAccountInformationMap : NopEntityTypeConfiguration<WalletAccountInformation>
    {
        public WalletAccountInformationMap()
        {
            ToTable("WalletAccountInformation", "Customer");
            HasKey(e => e.Id);

            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}