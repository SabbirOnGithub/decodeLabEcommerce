using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class MobileSettingsModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ActivatePushNotification")]
        public bool ActivatePushNotification { get; set; }
        public bool ActivatePushNotification_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.SandboxMode")]
        public bool SandboxMode { get; set; }
        public bool SandboxMode_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.GcmApiKey")]
        public string GcmApiKey { get; set; }
        public bool GcmApiKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.GoogleApiProjectNumber")]
        public string GoogleApiProjectNumber { get; set; }
        public bool GoogleApiProjectNumber_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.UploudeIOSPEMFile")]
        [UIHint("Download")]
        public int UploudeIOSPEMFile { get; set; }
        public bool UploudeIOSPEMFile_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.PEMPassword")]
        public string PEMPassword { get; set; }
        public bool PEMPassword_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppNameOnGooglePlayStore")]
        public string AppNameOnGooglePlayStore { get; set; }
        public bool AppNameOnGooglePlayStore_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppUrlOnGooglePlayStore")]
        public string AppUrlOnGooglePlayStore { get; set; }
        public bool AppUrlOnGooglePlayStore_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppNameOnAppleStore")]
        public string AppNameOnAppleStore { get; set; }
        public bool AppNameOnAppleStore_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppUrlonAppleStore")]
        public string AppUrlonAppleStore { get; set; }
        public bool AppUrlonAppleStore_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppDescription")]
        public string AppDescription { get; set; }
        public bool AppDescription_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppImage")]
        [UIHint("Picture")]
        public int AppImage { get; set; }
        public bool AppImage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppLogo")]
        [UIHint("Picture")]
        public int AppLogo { get; set; }
        public bool AppLogo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.AppLogoAltText")]
        public string AppLogoAltText { get; set; }
        public bool AppLogoAltText_OverrideForStore { get; set; }

    }
}