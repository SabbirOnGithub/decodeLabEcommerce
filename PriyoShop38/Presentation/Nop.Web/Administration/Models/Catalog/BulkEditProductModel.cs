using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using static Nop.Admin.Models.Catalog.ProductModel;

namespace Nop.Admin.Models.Catalog
{
    public partial class BulkEditProductModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.SKU")]
        [AllowHtml]
        public string Sku { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.OldPrice")]
        public decimal OldPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.ManageInventoryMethod")]
        public string ManageInventoryMethod { get; set; }

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        #region brainstation
        public string CategoryIds { get; set; }
        public string CategoryCount { get; set; }
        public decimal ProductCost { get; set; }

        //vendors
        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Vendor")]
        public int VendorId { get; set; }

        public string VendorName { get; set; }
        public ProductPictureModel ProductPictureModels { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }
        #endregion

        [NopResourceDisplayName("Admin.Catalog.BulkEdit.Fields.Published")]
        public bool Published { get; set; }

    }
}