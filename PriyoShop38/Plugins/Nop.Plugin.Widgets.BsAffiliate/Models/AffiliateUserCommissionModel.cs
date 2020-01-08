using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Web.Framework;
using System;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    public class AffiliateUserCommissionModel
    {
        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CommissionRate")]
        public decimal CommissionRate { get; set; }
        public string CommissionString { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CommissionType")]
        public CommissionType CommissionType { get; set; }
        public string CommissionTypeString { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.UpdatedOnUtc")]
        public DateTime UpdatedOnUtc { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.AffiliateName")]
        public string AffiliateName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.Id")]
        public int Id { get; set; }

        public string AffiliateEmail { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CustomerName")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.AffiliateType")]
        public string AffiliateType { get; set; }

        public string CustomerEmail { get; set; }
        public int CustomerId { get; set; }
        public string BKash { get; set; }
    }
}
