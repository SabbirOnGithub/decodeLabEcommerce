using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Collections.Generic;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.HomePageProduct.Services
{
    public partial class HomePageCategoryService : IHomePageCategoryService
    {
        #region Field
        private readonly IRepository<HomePageCategory> _homePageCategoryRepository;
        private readonly IRepository<HomePageProductCategoryImage> _homePageProductCategoryImageRepository;
        private readonly IEventPublisher _eventPublisher;
        #endregion

        #region Ctr

        public HomePageCategoryService(IRepository<HomePageCategory> homePageCategoryRepository,
            IRepository<HomePageProductCategoryImage> homePageProductCategoryImageRepository,
            IEventPublisher eventPublisher)
        {
            this._homePageCategoryRepository = homePageCategoryRepository;
            this._eventPublisher = eventPublisher;
            this._homePageProductCategoryImageRepository = homePageProductCategoryImageRepository;
        }

        #endregion

        #region Methods

        public void Delete(int categoryId)
        {
            var query = from c in _homePageCategoryRepository.Table
                        where c.CategoryId == categoryId
            select c;
            var homepageCategories = query.ToList();
            foreach (var homepageCategory in homepageCategories)
            {
                var images = _homePageProductCategoryImageRepository.Table.Where(x => x.CategoryId == homepageCategory.CategoryId).ToList();
                foreach (var image in images)
                {
                    _homePageProductCategoryImageRepository.Delete(image);
                }
                _homePageCategoryRepository.Delete(homepageCategory);
            }
        }

        public bool Update(HomePageCategory homePageCategory)
        {
            if (homePageCategory == null)
                throw new ArgumentNullException("customer");

            _homePageCategoryRepository.Update(homePageCategory);
            _eventPublisher.EntityUpdated(homePageCategory);

            return true;
        }

        public bool IsCategoryExist(int categoryId)
        {
            var query = from c in _homePageCategoryRepository.Table
                        where c.CategoryId == categoryId
                        select c;
            var homepageCategories = query.ToList();
            if (homepageCategories.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public HomePageCategory GetHomePageCategoryByCategoryId(int categoryId)
        {
            var query = from c in _homePageCategoryRepository.Table
                        where c.CategoryId == categoryId
                        select c;
            return query.FirstOrDefault();
        }

        public void Insert(HomePageCategory item)
        {
            //default value
            item.CreatedOnUtc= DateTime.UtcNow;
            item.UpdateOnUtc = DateTime.UtcNow;

            _homePageCategoryRepository.Insert(item);
        }

        public IPagedList<HomePageCategory> GetHomePageCategory(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from c in _homePageCategoryRepository.Table
                        select c;

            query = query.OrderBy(b => b.CategoryPriority);

            var homePageCategory = new PagedList<HomePageCategory>(query, pageIndex, pageSize);
            return homePageCategory;
        }

        #endregion    
    }
}