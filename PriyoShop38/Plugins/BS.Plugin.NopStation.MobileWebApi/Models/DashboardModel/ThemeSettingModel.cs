using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class ThemeSettingModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HeaderBackgroundColor")]
        public string HeaderBackgroundColor { get; set; }
        public bool HeaderBackgroundColor_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HeaderFontandIconColor")]
        public string HeaderFontandIconColor { get; set; }
        public bool HeaderFontandIconColor_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.HighlightedTextColor")]
        public string HighlightedTextColor { get; set; }
        public bool HighlightedTextColor_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.PrimaryTextColor")]
        public string PrimaryTextColor { get; set; }
        public bool PrimaryTextColor_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.SecondaryTextColor")]
        public string SecondaryTextColor { get; set; }
        public bool SecondaryTextColor_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButton")]
        public string BackgroundColorofPrimaryButton { get; set; }
        public bool BackgroundColorofPrimaryButton_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButton")]
        public string TextColorofPrimaryButton { get; set; }
        public bool TextColorofPrimaryButton_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButton")]
        public string BackgroundColorofSecondaryButton { get; set; }
        public bool BackgroundColorofSecondaryButton_OverrideForStore { get; set; }

    }
}