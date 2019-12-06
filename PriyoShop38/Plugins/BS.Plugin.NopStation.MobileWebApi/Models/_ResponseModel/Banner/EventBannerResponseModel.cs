using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner
{
    public class EventBannerResponseModel : GeneralResponseModel<IList<EventBannerResponseModel.EventBannerModel>>
    {
        public class EventBannerModel
        {
            public int Id { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string BannerName { get; set; }
            public int PictureId { get; set; }
            public string PictureUrl { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
