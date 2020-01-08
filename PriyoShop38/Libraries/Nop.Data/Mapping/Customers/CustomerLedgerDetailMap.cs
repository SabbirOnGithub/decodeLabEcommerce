using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public class CustomerLedgerDetailMap : NopEntityTypeConfiguration<CustomerLedgerDetail>
    {
        public CustomerLedgerDetailMap()
        {
            ToTable("CustomerLedgerDetail", "Customer");
            HasKey(e => e.SystemID);

            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            //Property(e => e.AmountDescription)
            //    .IsRequired()
            //    .HasMaxLength(4000);

            //Property(e => e.AmountType)
            //    .IsRequired()
            //    .HasMaxLength(10)
            //    .IsUnicode(false);

            //Property(e => e.LastAddedDate)
            //    .HasColumnType("datetime");
        }
    }
}