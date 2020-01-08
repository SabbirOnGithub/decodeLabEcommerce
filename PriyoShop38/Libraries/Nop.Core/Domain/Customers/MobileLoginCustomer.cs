using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public class MobileLoginCustomer : BaseEntity
    {
        public int CustomerId { get; set; }

        public string MobileNumber { get; set; }

        public string Token { get; set; }

        public bool IsTokenValid { get; set; }

        public DateTime TokenCreatedOnUtc { get; set; }


        public virtual Customer Customer { get; set; }
    }
}
