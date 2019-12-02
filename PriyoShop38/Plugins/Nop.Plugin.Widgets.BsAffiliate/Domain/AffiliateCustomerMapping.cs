using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.BsAffiliate.Domain
{
    public class AffiliateCustomerMapping : BaseEntity
    {
        public int AffiliateId { get; set; }

        public int CustomerId { get; set; }

        public int AffiliateTypeId { get; set; }
    }
}
