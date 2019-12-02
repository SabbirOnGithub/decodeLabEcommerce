using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Services
{
    public partial class HomePageSubCategoryService : IHomePageSubCategoryService
    {
        #region Field
        private readonly IRepository<HomePageSubCategory> _homePageSubCategoryRepository;

        #endregion

        #region Ctr

        public HomePageSubCategoryService(IRepository<HomePageSubCategory> homePageSubCategoryRepository)
        {
            _homePageSubCategoryRepository = homePageSubCategoryRepository;
        }

        #endregion

        #region Methods

        public void Delete(int subcategoryId)
        {
            //item.Deleted = true;

            var query = from c in _homePageSubCategoryRepository.Table
                        where c.SubCategoryId == subcategoryId
            select c;
            var homepageSubCategories = query.ToList();
            foreach (var homepageSubCategory in homepageSubCategories)
            {
                _homePageSubCategoryRepository.Delete(homepageSubCategory);
            }
        }

        public void Insert(HomePageSubCategory item)
        {
            //default value
            item.CreatedOnUtc= DateTime.UtcNow;
            item.UpdateOnUtc = DateTime.UtcNow;

            _homePageSubCategoryRepository.Insert(item);
        }

        #endregion    
   
    
    

        public IList<int> GetHomePageSubCategoryBySubCategoryIdList(int subCategoryId)
        {
            var query = from p in _homePageSubCategoryRepository.Table
                        where p.SubCategoryId == subCategoryId
                        orderby p.SubCategoryPriority
                        select p.SubCategoryId;

            var subCategories = query.ToList();
            return subCategories;
        }

        public IList<int> GetHomePageSubCategoryByCategoryIdList(int categoryId)
        {
            var query = from p in _homePageSubCategoryRepository.Table
                        where p.CategoryId == categoryId
                        orderby p.SubCategoryPriority
                        select p.SubCategoryId;

            var subCategories = query.ToList();
            return subCategories;
        }

        public HomePageSubCategory GetHomePageSubCategoryByCategory(int categoryId)
        {
            var query = from p in _homePageSubCategoryRepository.Table
                        where p.SubCategoryId == categoryId
                        select p;

            return query.FirstOrDefault();
        }

        public HomePageSubCategory GetHomePageSubCategoryBySubCategoryId(int subCategoryId)
        {

            HomePageSubCategory objOfHomePageSubCategory = new HomePageSubCategory();

            var query = from p in _homePageSubCategoryRepository.Table
                        where p.SubCategoryId == subCategoryId
                        orderby p.SubCategoryPriority
                        select p;
            var homePageSubCategory = query.ToList();
            if (homePageSubCategory.Count > 0)
            {
                objOfHomePageSubCategory.Id = homePageSubCategory.FirstOrDefault().Id;
                objOfHomePageSubCategory.SubCategoryId = homePageSubCategory.FirstOrDefault().SubCategoryId;
                objOfHomePageSubCategory.CategoryId = homePageSubCategory.FirstOrDefault().CategoryId;
                objOfHomePageSubCategory.SubCategoryPriority = homePageSubCategory.FirstOrDefault().SubCategoryPriority;
                objOfHomePageSubCategory.UpdateOnUtc = homePageSubCategory.FirstOrDefault().UpdateOnUtc;
                objOfHomePageSubCategory.TabName = homePageSubCategory.FirstOrDefault().TabName;
            }
            return objOfHomePageSubCategory;
        }
    }
}