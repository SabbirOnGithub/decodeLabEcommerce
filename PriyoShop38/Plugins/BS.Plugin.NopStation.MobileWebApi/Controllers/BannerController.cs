using System.Collections.Generic;
using System.Web.Http;
using Nop.Core;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using Newtonsoft.Json;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class BannerController : WebApiController
    {
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
