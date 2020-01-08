using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    public class AffiliateModel
    {

        [NopResourceDisplayName("Admin.Affiliates.Fields.ID")]
        public int Id { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.URL")]
        public string Url { get; set; }


        [NopResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
        [AllowHtml]
        public string FriendlyUrlName { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.Active")]
        public bool Active { get; set; }

        public AddressModel Address { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.BKash")]
        public string BKash { get; set; }

        public bool IsApplied { get; set; }

        public bool IsActive { get; set; }
    }
}
