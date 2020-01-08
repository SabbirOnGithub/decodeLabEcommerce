using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class CopyProductModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.Products.Copy.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.CopyImages")]
        public bool CopyImages { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.CopyAllPriceAttributes")]
        public bool CopyAllPriceAttributes { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Copy.ProductCost")]
        public decimal ProductCost { get; set; }
    }
}