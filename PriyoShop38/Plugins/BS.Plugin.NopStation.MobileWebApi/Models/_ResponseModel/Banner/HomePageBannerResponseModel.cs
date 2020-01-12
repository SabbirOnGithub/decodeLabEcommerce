using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner
{
    public class HomePageBannerResponseModel : GeneralResponseModel<IList<HomePageBannerResponseModel.BannerModel>>
    {
        public bool IsEnabled { get; set; }

        #region nested class

        public class BannerModel
        {
            public string ImageUrl { get; set; }

            public string Text { get; set; }

            public string Link { get; set; }

            public int DomainType { get; set; }

            public int DomainId { get; set; }


            public bool IsProduct { get; set; }

            public int ProdOrCatId { get; set; }
        }

        #endregion
    }
}
