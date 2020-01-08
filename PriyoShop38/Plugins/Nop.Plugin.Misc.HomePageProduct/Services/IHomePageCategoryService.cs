using System;
using Nop.Core;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Services
{
    public partial interface IHomePageCategoryService
    {
        void Delete(int categoryId);
        void Insert(HomePageCategory item);
        bool Update(HomePageCategory homePageCategory);
        bool IsCategoryExist(int categoryId);
        HomePageCategory GetHomePageCategoryByCategoryId(int categoryId);

        IPagedList<HomePageCategory> GetHomePageCategory(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}