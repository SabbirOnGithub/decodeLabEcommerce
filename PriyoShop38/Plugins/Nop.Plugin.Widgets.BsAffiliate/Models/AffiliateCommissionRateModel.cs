using FluentValidation.Attributes;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Extensions;
using Nop.Web.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    [Validator(typeof(CommissionRateValidator))]
    public class AffiliateCommissionRateModel
    {
        public AffiliateCommissionRateModel()
        {
            CategoryList = new SelectList(new List<SelectListItem>());
            VendorList = new SelectList(new List<SelectListItem>());
        }
        public int Id { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.CommissionRate.EntityName")]
        public string EntityName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.CommissionRate.EntityId")]
        public int? EntityId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.CommissionRate.Type")]
        public string EntityTypeString { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.CommissionRate.Type")]
        public EntityType? EntityType { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate")]
        public decimal CommissionRate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.CommissionRate.CommissionType")]
        public CommissionType? CommissionType { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.CommissionRate.VendorId")]
        public int? VendorId { get; set; }

        public SelectList VendorList { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.Affiliate.CommissionRate.CategoryId")]
        public int? CategoryId { get; set; }

        public SelectList CategoryList { get; set; }
        public string CommissionTypeString { get; set; }
        public string CommissionRateString { get; set; }
    }
}
