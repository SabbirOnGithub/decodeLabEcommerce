using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.AlgoliaSearch
{
    public class AlgoliaSettings : ISettings
    {
        public string ApplicationId { get; set; }
        public string SeachOnlyKey { get; set; }
        public string AdminKey { get; set; }
        public string MonitoringKey { get; set; }
        public int SearchBoxThumbnailSize { get; set; }
        public int PageSize { get; set; }
        public int MinimumQueryLength { get; set; }
        public bool UploadSoldOutProducts { get; set; }
        public bool HideSoldOutProducts { get; set; }
        public bool UploadUnPublishedProducts { get; set; }
        public int MaximumCategoriesShowInFilter { get; set; }
        public int MaximumVendorsShowInFilter { get; set; }
        public int MaximumManufacturersShowInFilter { get; set; }
        public int MaximumSpecificationsShowInFilter { get; set; }
        public bool AllowPriceRangeFilter { get; set; }
        public bool AllowEmiFilter { get; set; }
        public bool AllowRatingFilter { get; set; }
        public bool AllowVendorFilter { get; set; }
        public bool AllowManufacturerFilter { get; set; }
        public bool AllowSpecificationFilter { get; set; }
        public bool AllowCategoryFilter { get; set; }
        public string SelectablePageSizes { get; set; }
        public bool AllowProductSorting { get; set; }
        public bool AllowCustomersToSelectPageSize { get; set; }
        public bool AllowProductViewModeChanging { get; set; }
    }
}
