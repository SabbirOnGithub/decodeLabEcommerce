using Nop.Web.Framework;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models
{
    public class ConfigureModel
    {
        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.ApplicationId")]
        [Required]
        public string ApplicationId { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.SeachOnlyKey")]
        [Required]
        public string SeachOnlyKey { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AdminKey")]
        [Required]
        public string AdminKey { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.MonitoringKey")]
        [Required]
        public string MonitoringKey { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.ResetSettings")]
        public bool ResetSettings { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.ClearIndex")]
        public bool ClearIndex { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.SearchBoxThumbnailSize")]
        public int SearchBoxThumbnailSize { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.PageSize")]
        public int PageSize { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.MinimumQueryLength")]
        public int MinimumQueryLength { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.UploadSoldOutProducts")]
        public bool UploadSoldOutProducts { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.HideSoldOutProducts")]
        public bool HideSoldOutProducts { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.UploadUnPublishedProducts")]
        public bool UploadUnPublishedProducts { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.MaximumCategoriesShowInFilter")]
        public int MaximumCategoriesShowInFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.MaximumVendorsShowInFilter")]
        public int MaximumVendorsShowInFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.MaximumManufacturersShowInFilter")]
        public int MaximumManufacturersShowInFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.MaximumSpecificationsShowInFilter")]
        public int MaximumSpecificationsShowInFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowPriceRangeFilter")]
        public bool AllowPriceRangeFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowEmiFilter")]
        public bool AllowEmiFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowRatingFilter")]
        public bool AllowRatingFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowVendorFilter")]
        public bool AllowVendorFilter { get; set; }
        
        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowManufacturerFilter")]
        public bool AllowManufacturerFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowSpecificationFilter")]
        public bool AllowSpecificationFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowCategoryFilter")]
        public bool AllowCategoryFilter { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.SelectablePageSizes")]
        public string SelectablePageSizes { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowProductSorting")]
        public bool AllowProductSorting { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.Setting.AllowProductViewModeChanging")]
        public bool AllowProductViewModeChanging { get; set; }
    }
}
