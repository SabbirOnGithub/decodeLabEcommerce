using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.HomePageProduct.Models
{
    public partial class HomePageProductModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.SKU")]
        [AllowHtml]
        public string Sku { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.OldPrice")]
        public decimal OldPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.ManageInventoryMethod")]
        [UIHint("ManageInventoryTypes")]
        public string ManageInventoryMethod { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.DisableBuyButton")]
        public virtual bool DisableBuyButton { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ShowOnHomePage")]
        public virtual bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductCost")]
        public virtual decimal ProductCost { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Add")]
        public int Add { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Delete")]
        public int Delete { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        public int Priority { get; set; }
    }
}