using System;
using System.Web.Http;
using System.Web.Http.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Device;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Web.Framework.Controllers;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ThemeSetting;
using Nop.Services.Stores;
using Nop.Services.Configuration;
using Nop.Services.Media;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{


    public class KeepAliveApiController : WebApiController
    {
        
        #region Field
        private readonly IStoreService _storeService;
        private readonly IDeviceService _deviceService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        #endregion

        public KeepAliveApiController(IDeviceService deviceService, IWorkContext workContext, 
            IStoreService storeService,
            ISettingService settingService,
            IPictureService pictureService)
        {
            _deviceService = deviceService;
            _workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._pictureService = pictureService;
        }

        #region Utility
        private string GetPictureUrl(int pictureId)
        {
            var imageUrl = _pictureService.GetPictureUrl(pictureId, 300, showDefaultPicture: false);

            string ImageUrl = imageUrl == null ? string.Empty : imageUrl;

            return ImageUrl;
        }

        private ThemeSettingsResponseModel GetThemeSettings()
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

            return result;
        }

        #endregion


        [HttpGet]
        [Route("api/Check")]
        public IHttpActionResult ApiCheck()
        {
            var model = new { Version = 1, OldVersion = 0, NewApi = "api/", OldApi = "" };
            return Ok(model);
        }

        [HttpGet]
        [Route("api/ThemeCheck")]
        public IHttpActionResult ApiThemeCheck()
        {
            var model = new GeneralResponseModel<int>();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            model.Data = BsNopMobileSettings.BSMobSetVers;
            
            return Ok(model);
        }

        [HttpPost]
     //   [AcceptVerbs("GET", "POST")]
        [Route("api/AppStart")]
        public IHttpActionResult AppStart(AppStartModel queryModel)
        {
            var device = _deviceService.GetDeviceByDeviceToken(GetDeviceIdFromHeader());
            if (device != null)
            {
                device.CustomerId = _workContext.CurrentCustomer.Id;
                device.DeviceToken = GetDeviceIdFromHeader();
                device.SubscriptionId = queryModel.SubscriptionId;
                device.UpdatedOnUtc = DateTime.UtcNow;
                device.DeviceTypeId = queryModel.DeviceTypeId;
                device.IsRegistered = _workContext.CurrentCustomer.IsRegistered();
                _deviceService.UpdateDevice(device);
            }
            else
            {
                device = new Device
                {
                    CustomerId = _workContext.CurrentCustomer.Id,
                    DeviceToken = GetDeviceIdFromHeader(),
                    SubscriptionId = queryModel.SubscriptionId,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                    DeviceTypeId = queryModel.DeviceTypeId,
                    IsRegistered = _workContext.CurrentCustomer.IsRegistered()
                };

                _deviceService.InsertDevice(device);
            }

            var result = GetThemeSettings();

            return Ok(result);
        }


    }
}
