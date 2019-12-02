using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Catalog
{
    public class MegaMenuModel
    {
        public MegaMenuModel()
        {
            Categories = new List<CategoryModel>();
        }

        public IList<CategoryModel> Categories { get; set; }

        public class CategoryModel : BaseNopEntityModel
        {
            public CategoryModel()
            {
                ChildCategories = new List<CategoryModel>();
            }

            public string Name { get; set; }

            public string SeName { get; set; }

            public IList<CategoryModel> ChildCategories { get; set; }
        }
    }
}