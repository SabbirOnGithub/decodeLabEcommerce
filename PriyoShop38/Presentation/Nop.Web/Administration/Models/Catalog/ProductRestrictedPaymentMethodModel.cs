using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public class ProductRestrictedPaymentMethodModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.IsActive")]
        public bool IsActive { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Payment.Methods.Fields.Logo")]
        public string LogoUrl { get; set; }


        [NopResourceDisplayName("Admin.Configuration.Products.RestrictedPaymentMethod.IsRestricted")]
        public bool IsRestricted { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Products.RestrictedPaymentMethod.ProductId")]
        public int ProductId { get; set; }
    }
}