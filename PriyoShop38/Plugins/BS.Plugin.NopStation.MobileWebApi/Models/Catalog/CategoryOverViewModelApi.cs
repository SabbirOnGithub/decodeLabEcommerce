using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class CategoryOverViewModelApi : CategoryBaseModelApi
    {
        public CategoryOverViewModelApi()
        {
            DefaultPictureModel = new PictureModel();
        }
        public PictureModel DefaultPictureModel ;
    }
}
