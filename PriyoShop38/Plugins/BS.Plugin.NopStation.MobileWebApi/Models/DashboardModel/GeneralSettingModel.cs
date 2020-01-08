using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class GeneralSettingModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.EnableBestseller")]
        public bool EnableBestseller { get; set; }
        public bool EnableBestseller_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.EnableFeaturedProducts")]
        public bool EnableFeaturedProducts { get; set; }
        public bool EnableFeaturedProducts_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.EnableNewProducts")]
        public bool EnableNewProducts { get; set; }
        public bool EnableNewProducts_OverrideForStore { get; set; }

        public partial class AssociatedProductModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Catalog.Products.AssociatedProducts.Fields.Product")]
            public string ProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.AssociatedProducts.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.Fields.Published")]
            public bool Published { get; set; }
        }
        public partial class AddAssociatedProductModel : BaseNopModel
        {
            public AddAssociatedProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int ProductId { get; set; }

            public int[] SelectedProductIds { get; set; }

            //vendor
            public bool IsLoggedInAsVendor { get; set; }
        }

    }
}