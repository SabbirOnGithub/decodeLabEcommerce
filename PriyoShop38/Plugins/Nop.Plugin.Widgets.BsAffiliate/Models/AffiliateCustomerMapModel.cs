using Nop.Web.Framework;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    public class AffiliateCustomerMapModel
    {
        public AffiliateCustomerMapModel()
        {
            AvailableAffiliateTypes = new List<SelectListItem>();
        }

        public int AffiliateId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerName")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateName")]
        public string AffiliateName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateTypeId")]
        public int AffiliateTypeId { get; set; }

        public List<SelectListItem> AvailableAffiliateTypes { get; set; }

        public string FirstName { get; set; }
        public string LaststName { get; set; }
        public string Email { get; set; }
    }
}
