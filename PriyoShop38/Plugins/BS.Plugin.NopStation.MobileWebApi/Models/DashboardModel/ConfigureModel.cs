using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class ConfigureModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppName")]
        public string AppName { get; set; }
        public bool AppName_OverrideForStore { get; set; }
     
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppKey")]
        public string AppKey { get; set; }
        public bool AppKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.LicenceType")]
        public string LicenceType { get; set; }
        public bool LicenceType_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.CreatedDate")]
        public string CreatedDate { get; set; }
        public bool CreatedDate_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AndroidAppStatus")]
        public string AndroidAppStatus { get; set; }
        public bool AndroidAppStatus_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.DownloadUrl")]
        public string DownloadUrl { get; set; }
        public bool DownloadUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.iOsAPPUDID")]
        public string iOsAPPUDID { get; set; }
        public bool iOsAPPUDID_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.MobilWebsiteURL")]
        public string MobilWebsiteURL { get; set; }
        public bool MobilWebsiteURL_OverrideForStore { get; set; }

      
    }
}