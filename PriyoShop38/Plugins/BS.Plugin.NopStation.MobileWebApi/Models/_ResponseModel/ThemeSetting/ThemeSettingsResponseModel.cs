using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ThemeSetting
{
    public class ThemeSettingsResponseModel : GeneralResponseModel<BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ThemeSetting.ThemeSettingsResponseModel.ThemeSettingsModel>
    {
        public ThemeSettingsResponseModel()
        {
            
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        #region Nested Class

        public class ThemeSettingsModel
        {
            public ThemeSettingsModel()
            {
                BannerModel = new List<BannerModel>();
            }

            public List<BannerModel> BannerModel { get; set; }

            public string AppImgUrl { get; set; }
            public string AppLogoUrl { get; set; }
            public string GcmApiKey { get; set; }
            public string AppName { get; set; }
            public string AppKey { get; set; }
            public string LicenceType { get; set; }
            public string CreatedDate { get; set; }
            public string AndroidAppStatus { get; set; }
            public string DownloadUrl { get; set; }
            public string iOsAPPUDID { get; set; }
            public string MobilWebsiteURL { get; set; }
            public string HeaderBackgroundColor { get; set; }
            public string HeaderFontandIconColor { get; set; }
            public string HighlightedTextColor { get; set; }
            public string PrimaryTextColor { get; set; }
            public string SecondaryTextColor { get; set; }
            public string BackgroundColorofPrimaryButton { get; set; }
            public string TextColorofPrimaryButton { get; set; }
            public string BackgroundColorofSecondaryButton { get; set; }
            public bool DefaultNopFlowSameAs { get; set; }
            public string PushNotificationHeading { get; set; }
            public string PushNotificationMessage { get; set; }
            public bool Willusedefaultnopcategory { get; set; }
            public bool ActivatePushNotification { get; set; }
            public bool SandboxMode { get; set; }
            public string GoogleApiProjectNumber { get; set; }
            public int UploudeIOSPEMFile { get; set; }
            public string PEMPassword { get; set; }
            public string AppNameOnGooglePlayStore { get; set; }
            public string AppUrlOnGooglePlayStore { get; set; }
            public string AppNameOnAppleStore { get; set; }
            public string AppUrlonAppleStore { get; set; }
            public string AppDescription { get; set; }
            public int AppImage { get; set; }
            public int AppLogo { get; set; }
            public string AppLogoAltText { get; set; }
            public bool EnableBestseller { get; set; }
            public bool EnableFeaturedProducts { get; set; }
            public bool EnableNewProducts { get; set; }
        }


        public class BannerModel
        {
            public int PictureId { get; set; }
            public string Text { get; set; }
            public string Link { get; set; }
            public bool IsProduct { get; set; }
            public string ProductOrCategoryId { get; set; }
        }
        #endregion
    }

    public class ThemeVersionResponseModel : GeneralResponseModel<int>
    {

    }
}
