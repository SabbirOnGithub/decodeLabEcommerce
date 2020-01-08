using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ThemeSetting;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ThemeSettingsController : WebApiController
    {
        #region Field
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        #endregion

        #region Ctor
        public ThemeSettingsController(IPluginFinder pluginFinder,
            IStoreService storeService,
            IWorkContext workContext,
            ISettingService settingService,
            IPictureService pictureService)
        {
            this._pluginFinder = pluginFinder;
            this._storeService = storeService;
            this._workContext = workContext;
            this._settingService = settingService;
            this._pictureService = pictureService;
        }

        #endregion

        #region Utility

        private string GetPictureUrl(int pictureId)
        {
            var imageUrl = _pictureService.GetPictureUrl(pictureId, 300, showDefaultPicture: false);

            string ImageUrl = imageUrl == null ? string.Empty : imageUrl;

            return ImageUrl;
        }
        #endregion

        #region Action Method

        [Route("api/themesettings")]
        [HttpGet]
        public IHttpActionResult Theme()
        {
            var result = new ThemeSettingsResponseModel();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            if (BsNopMobileSettings != null)
            {
                ThemeSettingsResponseModel.ThemeSettingsModel model = new ThemeSettingsResponseModel.ThemeSettingsModel();

                model.HeaderBackgroundColor = BsNopMobileSettings.HeaderBackgroundColor;
                model.HeaderFontandIconColor = BsNopMobileSettings.HeaderFontandIconColor;
                model.HighlightedTextColor = BsNopMobileSettings.HighlightedTextColor;
                model.PrimaryTextColor = BsNopMobileSettings.PrimaryTextColor;
                model.SecondaryTextColor = BsNopMobileSettings.SecondaryTextColor;
                model.BackgroundColorofPrimaryButton = BsNopMobileSettings.BackgroundColorofPrimaryButton;
                model.TextColorofPrimaryButton = BsNopMobileSettings.TextColorofPrimaryButton;
                model.BackgroundColorofSecondaryButton = BsNopMobileSettings.BackgroundColorofSecondaryButton;
                model.AppImage = BsNopMobileSettings.AppImage;
                model.AppLogo = BsNopMobileSettings.AppLogo;
                model.AppLogoAltText = BsNopMobileSettings.AppLogoAltText;
                model.AppImgUrl = GetPictureUrl(BsNopMobileSettings.AppImage);
                model.AppLogoUrl = GetPictureUrl(BsNopMobileSettings.AppLogo);
                model.BannerModel = new List<ThemeSettingsResponseModel.BannerModel>();

                #region Banner1

                ThemeSettingsResponseModel.BannerModel bModel1 = new ThemeSettingsResponseModel.BannerModel();

                bModel1.PictureId = BsNopMobileSettings.Picture1Id;
                bModel1.Text = BsNopMobileSettings.Text1;
                bModel1.Link = BsNopMobileSettings.Link1;
                bModel1.IsProduct = BsNopMobileSettings.IsProduct1;
                bModel1.ProductOrCategoryId = BsNopMobileSettings.ProductOrCategoryId1;

                model.BannerModel.Add(bModel1);

                #endregion

                #region Banner2

                ThemeSettingsResponseModel.BannerModel bModel2 = new ThemeSettingsResponseModel.BannerModel();

                bModel2.PictureId = BsNopMobileSettings.Picture2Id;
                bModel2.Text = BsNopMobileSettings.Text2;
                bModel2.Link = BsNopMobileSettings.Link2;
                bModel2.IsProduct = BsNopMobileSettings.IsProduct2;
                bModel2.ProductOrCategoryId = BsNopMobileSettings.ProductOrCategoryId2;

                model.BannerModel.Add(bModel2);

                #endregion
                #region Banner3

                ThemeSettingsResponseModel.BannerModel bModel3 = new ThemeSettingsResponseModel.BannerModel();

                bModel3.PictureId = BsNopMobileSettings.Picture3Id;
                bModel3.Text = BsNopMobileSettings.Text3;
                bModel3.Link = BsNopMobileSettings.Link3;
                bModel3.IsProduct = BsNopMobileSettings.IsProduct3;
                bModel3.ProductOrCategoryId = BsNopMobileSettings.ProductOrCategoryId3;

                model.BannerModel.Add(bModel3);

                #endregion
                #region Banner4

                ThemeSettingsResponseModel.BannerModel bModel4 = new ThemeSettingsResponseModel.BannerModel();

                bModel4.PictureId = BsNopMobileSettings.Picture4Id;
                bModel4.Text = BsNopMobileSettings.Text4;
                bModel4.Link = BsNopMobileSettings.Link4;
                bModel4.IsProduct = BsNopMobileSettings.IsProduct4;
                bModel4.ProductOrCategoryId = BsNopMobileSettings.ProductOrCategoryId4;

                model.BannerModel.Add(bModel4);

                #endregion
                #region Banner5

                ThemeSettingsResponseModel.BannerModel bModel5 = new ThemeSettingsResponseModel.BannerModel();

                bModel5.PictureId = BsNopMobileSettings.Picture5Id;
                bModel5.Text = BsNopMobileSettings.Text5;
                bModel5.Link = BsNopMobileSettings.Link5;
                bModel5.IsProduct = BsNopMobileSettings.IsProduct5;
                bModel5.ProductOrCategoryId = BsNopMobileSettings.ProductOrCategoryId5;

                model.BannerModel.Add(bModel5);

                #endregion

                result.ActiveStoreScopeConfiguration = storeScope;

                // result.Data = BsNopMobileSettings;
                result.Data = model;
            }
            else
            {
                result.ErrorList.Add("Error Occurred");
            }

            return Ok(result);
        }

        #endregion
    }
}
