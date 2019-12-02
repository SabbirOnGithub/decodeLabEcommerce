using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CountryController: WebApiController
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        
        #endregion

        #region Constructors

        
        public CountryController(ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            
        }

        #endregion

        #region States / provinces

        [Route("api/country/getstatesbycountryid/{countryId}")]
        [HttpGet]
        public IHttpActionResult GetStatesByCountryId(string countryId, bool addSelectStateItem=false)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(countryId))
                throw new ArgumentNullException("countryId");

            string cacheKey = string.Format(ModelCacheEventConsumer.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, countryId, addSelectStateItem, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
                var states = _stateProvinceService.GetStateProvincesByCountryId(country != null ? country.Id : 0).ToList();
                var result = (from s in states
                              select new { id = s.Id, name = s.GetLocalized(x => x.Name) })
                              .ToList();


                if (country == null)
                {
                    //country is not selected ("choose country" item)
                    if (addSelectStateItem)
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                    }
                    else
                    {
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                }
                else
                {
                    //some country is selected
                    if (result.Count == 0)
                    {
                        //country does not have states
                        result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                    }
                    else
                    {
                        //country has some states
                        if (addSelectStateItem)
                        {
                            result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.SelectState") });
                        }
                    }
                }

                return result;
            });

            var  response = new GeneralResponseModel<object>()
            {
                Data = cacheModel
            };
            return Ok(response);
            //return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
