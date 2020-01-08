using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Core;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public interface IBS_HomePageCategoryService
    {
        void InsertHomePageCategory(BS_HomePageCategory homePageCategory);

        void UpdateHomePageCategory(BS_HomePageCategory homePageCategory);

        void DeleteHomePageCategory(BS_HomePageCategory homePageCategory);

        BS_HomePageCategory GetHomePageCategoryById(int homePageCategoryId);

        BS_HomePageCategory GetHomePageCategoryByCategoryId(int categoryId);

        IPagedList<BS_HomePageCategory> GetAllHomePageCategories(string keyword = "", List<int> categoryIds = null,
            bool? publish = null, int pageIndex = 0, int pageSize = int.MaxValue - 1);


        void InsertHomePageCategoryProduct(BS_HomePageCategoryProduct homePageCategoryProduct);

        void UpdateHomePageCategoryProduct(BS_HomePageCategoryProduct homePageCategoryProduct);

        void DeleteHomePageCategoryProduct(BS_HomePageCategoryProduct homePageCategoryProduct);

        BS_HomePageCategoryProduct GetHomePageCategoryProductById(int homePageCategoryProductId);

        BS_HomePageCategoryProduct GetHomePageCategoryProductByProductId(int homePageCategoryId, int productId);
    }
}