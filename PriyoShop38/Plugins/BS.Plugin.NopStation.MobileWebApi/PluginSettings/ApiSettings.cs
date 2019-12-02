using Nop.Core.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi.PluginSettings
{
    public class ApiSettings : ISettings
    {
        public int BogoCategoryId { get; set; }
        public int AppOnlyCategoryId { get; set; }
        public int Offer99CategoryId { get; set; }
        public int OmgSaleCategoryId { get; set; }


        public int CartThumbPictureSize { get; set; }
        public bool ShowHomePageTopCategoryListIcon { get; set; }
        public int CategoryListIconId { get; set; }
        public bool ShowHomePageTopManufacturersIcon { get; set; }
        public int ManufacturerListIconId { get; set; }
    }
}
