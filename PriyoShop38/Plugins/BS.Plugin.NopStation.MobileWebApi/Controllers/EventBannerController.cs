using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class EventBannerController : WebApiController
    {
        #region Fields

        private readonly IEventBannerService _eventBannerService;
        private readonly ICategoryService _categoryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region ctor

        public EventBannerController(IEventBannerService eventBannerService,
            IStateProvinceService stateProvinceService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager, ICategoryService categoryService)
        {
            _eventBannerService = eventBannerService;
            _stateProvinceService = stateProvinceService;
            _localizationService = localizationService;
            _workContext = workContext;
            _cacheManager = cacheManager;
            _categoryService = categoryService;
        }

        #endregion

        #region methods

        [Route("api/EventBanner")]
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            var eventBannerList = _eventBannerService.GetAllEventBanners();
            var eventBannerVmList = new List<EventBannerResponseModel.EventBannerModel>();

            if (eventBannerList.Any())
                foreach (var eventBanner in eventBannerList)
                {
                    var eventBannerVm = new EventBannerResponseModel.EventBannerModel();
                    eventBannerVm.BannerName = eventBanner.BannerName;
                    eventBannerVm.CategoryId = eventBanner.CategoryId;
                    eventBannerVm.Id = eventBanner.Id;
                    eventBannerVm.CategoryName = _categoryService.GetCategoryById(eventBanner.CategoryId).Name;
                    eventBannerVm.PictureId = eventBanner.PictureId;
                    eventBannerVm.PictureUrl = GetRootUrl() + "content/images/eventBanner/" + eventBanner.PictureUrl;
                    eventBannerVmList.Add(eventBannerVm);
                }

            var result = new EventBannerResponseModel();
            result.Data = eventBannerVmList;
            return Ok(result);
        }
        #endregion


        public static string GetRootUrl()
        {
            var tmpURi = HttpContext.Current.Request.Url;
            var tmpPort = tmpURi.Port > 0 && tmpURi.Port != 80 ? ":" + tmpURi.Port : "";
            var rootUrl = tmpURi.Scheme + "://" + tmpURi.Host + tmpPort + "/";
            return rootUrl;
        }

        
    }
}