using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.BsMegaMenu.Models
{
    public class CategoryMenuModel : BaseNopModel
    {
        public CategoryMenuModel()
        {
            CategoryFeatureProductModel = new List<ProductOverviewModel>();
            FirstColCategoryList = new List<CategoryMenuModel>();
            SecondColCategoryList = new List<CategoryMenuModel>();
            ThirdColCategoryList = new List<CategoryMenuModel>();
            FourthColCategoryList = new List<CategoryMenuModel>();
        }

        public int Id { get; set; }
        public int MenuColumnNumber { get; set; }
        public string Name { get; set; }
        public string SeName { get; set; }
        public string PictureLink { get; set; }
        public List<ProductOverviewModel> CategoryFeatureProductModel { get; set; }
        public List<CategoryMenuModel> SubCategories { get; set; }
        public List<CategoryMenuModel> SubSubCategories { get; set; }
        public List<CategoryMenuModel> SubSubSubCategories { get; set; }
        public List<CategoryMenuModel> FirstColCategoryList { get; set; }
        public List<CategoryMenuModel> SecondColCategoryList { get; set; }
        public List<CategoryMenuModel> ThirdColCategoryList { get; set; }
        public List<CategoryMenuModel> FourthColCategoryList { get; set; }
    }
}