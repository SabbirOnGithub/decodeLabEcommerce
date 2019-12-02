using Nop.Core;
using Nop.Core.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Services.Events;
using System;
using System.Linq;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial class CategoryIconService : ICategoryIconService
    {
        #region Fields

        private readonly IRepository<BS_CategoryIcon> _categoryIconRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor
                
        public CategoryIconService(IRepository<BS_CategoryIcon> categoryIconRepository,
            IEventPublisher eventPublisher)
        {
            this._categoryIconRepository = categoryIconRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods
                
        public IPagedList<BS_CategoryIcon> GetAllCategoryIcons(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from ci in _categoryIconRepository.Table                        
                        select ci;

            query = query.OrderBy(x => x.DisplayOrder);

            return new PagedList<BS_CategoryIcon>(query, pageIndex, pageSize);
        }

        public BS_CategoryIcon GetCategoryIconByCategoryId(int categoryId)
        {
            var categoryIcon = (from ci in _categoryIconRepository.Table
                                where ci.CategoryId == categoryId
                                select ci).FirstOrDefault();

            return categoryIcon;
        }

        public BS_CategoryIcon GetCategoryIconById(int id)
        {
            var categoryIcon = _categoryIconRepository.GetById(id);

            return categoryIcon;
        }

        public void InsertCategoryIcon(BS_CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException("CategoryIcons");

            _categoryIconRepository.Insert(categoryIcon);

            //event notification
            _eventPublisher.EntityInserted(categoryIcon);
        }

        public void UpdateCategoryIcon(BS_CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException("CategoryIcons");

            _categoryIconRepository.Update(categoryIcon);

            //event notification
            _eventPublisher.EntityUpdated(categoryIcon);
        }

        public void DeleteCategoryIcon(BS_CategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException("CategoryIcons");

            _categoryIconRepository.Delete(categoryIcon);

            //event notification
            _eventPublisher.EntityDeleted(categoryIcon);
        }

        #endregion

    }
}
