using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Affiliates
{
    public class CategoryCommissionModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public bool Published { get; set; }

        public decimal CommissionRate { get; set; }
    }
}