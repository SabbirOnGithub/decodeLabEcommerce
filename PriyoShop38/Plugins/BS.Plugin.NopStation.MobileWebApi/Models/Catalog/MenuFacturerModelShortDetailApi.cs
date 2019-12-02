using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class MenuFacturerModelShortDetailApi 
    {
        public MenuFacturerModelShortDetailApi()
        {
            PictureModel = new PictureModel();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public PictureModel PictureModel { get; set; }
    }
}
