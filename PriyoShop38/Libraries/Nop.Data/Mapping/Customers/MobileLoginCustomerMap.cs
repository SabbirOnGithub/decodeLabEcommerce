using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public class MobileLoginCustomerMap : NopEntityTypeConfiguration<MobileLoginCustomer>
    {
        public MobileLoginCustomerMap()
        {
            this.ToTable("MobileLoginCustomer");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId);
        }
    }
}
