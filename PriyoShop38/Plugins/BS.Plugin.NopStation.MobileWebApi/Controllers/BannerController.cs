using System.Collections.Generic;
using System.Web.Http;
using Nop.Core;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using Newtonsoft.Json;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class BannerController : WebApiController
    {
        #region Fields

        private readonly IEventBannerService _eventBannerService;
        private readonly ICategoryService _categoryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors


        public BannerController(IEventBannerService eventBannerService,
            IStateProvinceService stateProvinceService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager, ICategoryService categoryService)
        {
            this._eventBannerService = eventBannerService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            _categoryService = categoryService;
        }

        #endregion

        #region Action Method

        [Route("api/homepagebanner")]
        [HttpGet]
        public IHttpActionResult HomePageBanner()
        {
            string jsonMenuCategory = null;
            var filePath = CommonHelper.MapPath("~/ApiJson/bs-slider-json.json");

            try
            {
                jsonMenuCategory = System.IO.File.ReadAllText(filePath);
            }
            catch { }

            if (string.IsNullOrWhiteSpace(jsonMenuCategory))
            {
                try
                {
                    filePath = CommonHelper.MapPath("~/ApiJson/bs-slider-backup-json.json");
                    jsonMenuCategory = System.IO.File.ReadAllText(filePath);
                }
                catch { }
            }
            var pictureList = JsonConvert.DeserializeObject<List<HomePageBannerResponseModel.BannerModel>>(jsonMenuCategory);
            

            var result = new HomePageBannerResponseModel();

            result.IsEnabled = pictureList.Count > 0;

            result.Data = pictureList;

            return Ok(result);
        }

        #endregion
    }
}
