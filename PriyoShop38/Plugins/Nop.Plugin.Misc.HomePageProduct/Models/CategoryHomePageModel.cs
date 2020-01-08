using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.HomePageProduct.Models
{
    public class CategoryHomePageModel : BaseNopModel
    {
        public CategoryHomePageModel()
        {
            Categories = new List<CategorySimpleModel>();
            Products = new List<ProductOverviewModel>();
            //PictureModel = new List<PictureModel>();
            PictureModel = new List<CustomPictureModel>();
        }
        public string MainCategoryName { get; set; }
        public string MainCategorySeName { get; set; }
        public string CategoryColor { get; set; }
        public List<CategorySimpleModel> Categories { get; set; }
        public List<ProductOverviewModel> Products { get; set; }
        //public List<PictureModel> PictureModel { get; set; }
        public List<CustomPictureModel> PictureModel { get; set; }
    }
}
