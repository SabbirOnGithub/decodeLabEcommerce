using System;
using Nop.Core;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Services
{
    public partial interface IHomePageSubCategoryService
    {
        void Delete(int categoryId);
        void Insert(HomePageSubCategory item);

        IList<int> GetHomePageSubCategoryBySubCategoryIdList(int subCategoryId);
        IList<int> GetHomePageSubCategoryByCategoryIdList(int categoryId);
        HomePageSubCategory GetHomePageSubCategoryByCategory(int categoryId);
        HomePageSubCategory GetHomePageSubCategoryBySubCategoryId(int subCategoryId);
    }
}