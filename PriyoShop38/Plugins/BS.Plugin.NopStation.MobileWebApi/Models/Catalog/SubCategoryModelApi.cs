using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class SubCategoryModelApi
    {
        public SubCategoryModelApi()
        {
            PictureModel = new PictureModel();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public PictureModel PictureModel { get; set; }
    }
}
