using Nop.Core.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public class MobileWebApiSettings : ISettings
    {
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
        public int Picture1Id { get; set; }
        public string Text1 { get; set; }
        public string Link1 { get; set; }
        public bool IsProduct1 { get; set; }
        public string ProductOrCategoryId1 { get; set; }

        public int Picture2Id { get; set; }
        public string Text2 { get; set; }
        public string Link2 { get; set; }        
        public bool IsProduct2 { get; set; }
        public string ProductOrCategoryId2 { get; set; }

        public int Picture3Id { get; set; }
        public string Text3 { get; set; }
        public string Link3 { get; set; }
        public bool IsProduct3 { get; set; }
        public string ProductOrCategoryId3 { get; set; }

        public int Picture4Id { get; set; }
        public string Text4 { get; set; }
        public string Link4 { get; set; }
        public bool IsProduct4 { get; set; }
        public string ProductOrCategoryId4 { get; set; }

        public int Picture5Id { get; set; }
        public string Text5 { get; set; }
        public string Link5 { get; set; }
        public bool IsProduct5 { get; set; }
        public string ProductOrCategoryId5 { get; set; }
        public string MobContactNumber { get; set; }
        public string MobContactEmail { get; set; }
        public int BSMobSetVers { get; set; }
    }
}
