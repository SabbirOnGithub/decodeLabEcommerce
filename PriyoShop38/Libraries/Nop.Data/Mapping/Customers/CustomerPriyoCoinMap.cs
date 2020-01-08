using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerPriyoCoinMap : NopEntityTypeConfiguration<CustomerPriyoCoin>
    {
        public CustomerPriyoCoinMap()
        {
            this.ToTable("CustomerPriyoCoin");
            this.HasKey(c => c.Id);
        }
    }
}
