using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Affiliates
{
    public class AffiliateCommissionModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public bool Active { get; set; }

        public decimal CommissionRate { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }
    }
}