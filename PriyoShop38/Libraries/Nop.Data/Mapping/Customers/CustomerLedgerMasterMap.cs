using System.ComponentModel.DataAnnotations.Schema;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public class CustomerLedgerMasterMap : NopEntityTypeConfiguration<CustomerLedgerMaster>
    {
        public CustomerLedgerMasterMap()
        {
            ToTable("CustomerLedgerMaster", "Customer");
            HasKey(e => e.SystemID);

            Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.CustomerName).IsOptional();


            //Property(e => e.SystemID).HasColumnName("SystemID");
            //Property(e => e.CreditAmount).HasColumnType("decimal(18, 2)");
            //Property(e => e.CustomerName).HasMaxLength(1000);
            //Property(e => e.DebitAmount).HasColumnType("decimal(18, 2)");
            //Property(e => e.LastUpdated).HasColumnType("datetime");
            //Property(e => e.TotalBalance).HasColumnType("decimal(18, 2)");
        }
    }
}