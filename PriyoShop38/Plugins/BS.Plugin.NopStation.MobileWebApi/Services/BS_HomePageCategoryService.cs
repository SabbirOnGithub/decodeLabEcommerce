using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Core;
using Nop.Core.Data;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class BS_HomePageCategoryService : IBS_HomePageCategoryService
    {
        #region Field

        private readonly IRepository<BS_HomePageCategory> _homePageCategoryRepository;
        private readonly IRepository<BS_HomePageCategoryProduct> _homePageCategoryProductRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctr

        public BS_HomePageCategoryService(IRepository<BS_HomePageCategory> homePageCategoryRepository,
            IRepository<BS_HomePageCategoryProduct> homePageCategoryProductRepository,
            IEventPublisher eventPublisher)
        {
            this._homePageCategoryRepository = homePageCategoryRepository;
            this._homePageCategoryProductRepository = homePageCategoryProductRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public void InsertHomePageCategory(BS_HomePageCategory homePageCategory)
        {
            _homePageCategoryRepository.Insert(homePageCategory);
        }

        public void UpdateHomePageCategory(BS_HomePageCategory homePageCategory)
        {
            _homePageCategoryRepository.Update(homePageCategory);
        }

        public void DeleteHomePageCategory(BS_HomePageCategory homePageCategory)
        {
            _homePageCategoryRepository.Delete(homePageCategory);
        }

        public BS_HomePageCategory GetHomePageCategoryById(int homePageCategoryId)
        {
            return _homePageCategoryRepository.GetById(homePageCategoryId);
        }

        public BS_HomePageCategory GetHomePageCategoryByCategoryId(int categoryId)
        {
            return _homePageCategoryRepository.Table.FirstOrDefault(x=> x.CategoryId == categoryId);
        }

        public IPagedList<BS_HomePageCategory> GetAllHomePageCategories(string keyword = "", List<int> categoryIds = null, 
            bool? publish = null, int pageIndex = 0, int pageSize = int.MaxValue - 1)
        {
            var query = _homePageCategoryRepository.Table;

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.TextPrompt.Contains(keyword));
            if (publish.HasValue)
                query = query.Where(x => x.Published == publish.Value);
            if (categoryIds != null && categoryIds.Count > 0)
                query = query.Where(x => categoryIds.Contains(x.CategoryId));

            query = query.OrderBy(x => x.DisplayOrder);

            return new PagedList<BS_HomePageCategory>(query, pageIndex, pageSize);
        }


        public void InsertHomePageCategoryProduct(BS_HomePageCategoryProduct homePageCategoryProduct)
        {
            _homePageCategoryProductRepository.Insert(homePageCategoryProduct);
        }

        public void UpdateHomePageCategoryProduct(BS_HomePageCategoryProduct homePageCategoryProduct)
        {
            _homePageCategoryProductRepository.Update(homePageCategoryProduct);
        }

        public void DeleteHomePageCategoryProduct(BS_HomePageCategoryProduct homePageCategoryProduct)
        {
            _homePageCategoryProductRepository.Delete(homePageCategoryProduct);
        }

        public BS_HomePageCategoryProduct GetHomePageCategoryProductById(int homePageCategoryProductId)
        {
            return _homePageCategoryProductRepository.GetById(homePageCategoryProductId);
        }

        public BS_HomePageCategoryProduct GetHomePageCategoryProductByProductId(int homePageCategoryId, int productId)
        {
            return _homePageCategoryProductRepository.Table.FirstOrDefault(x => x.ProductId == productId && x.HomePageCategoryId == homePageCategoryId);
        }

        #endregion
    }
}
