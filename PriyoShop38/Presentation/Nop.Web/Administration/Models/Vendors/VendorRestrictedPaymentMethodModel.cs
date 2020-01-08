using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Web.Mvc;

namespace Nop.Admin.Models.Vendors
{
    public partial class VendorRestrictedPaymentMethodModel : BaseNopEntityModel
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
        public int VendorId { get; set; }
    }
}