using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Stores;
using Nop.Services.Events;

namespace Nop.Services.Common
{
    public partial class EventBannerService: IEventBannerService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : show hidden records?
        /// </remarks>
        private const string EVENTBANNER_ALL_KEY = "Nop.eventBanner.all-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string EVENTBANNER_PATTERN_KEY = "Nop.eventBanner.";

        #endregion

        private readonly IRepository<EventBanner> _eventBannerRepository;
        private readonly IStoreContext _storeContext;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        public EventBannerService(ICacheManager cacheManager,
            IRepository<EventBanner> eventBannerRepository,
            IStoreContext storeContext,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._eventBannerRepository = eventBannerRepository;
            this._storeContext = storeContext;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }

        public void DeleteEventBanner(EventBanner eventBanner)
        {
            if (eventBanner == null)
                throw new ArgumentNullException("eventBanner");

            _eventBannerRepository.Delete(eventBanner);

            _cacheManager.RemoveByPattern(EVENTBANNER_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(eventBanner);
        }

        public IList<EventBanner> GetAllEventBanners(int languageId = 0, bool showHidden = false)
        {
            string key = string.Format(EVENTBANNER_ALL_KEY, languageId, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _eventBannerRepository.Table;

                var eventBanners = query.ToList();

                if (languageId > 0)
                {
                    eventBanners = eventBanners.OrderBy(x =>x.BannerName).ToList();
                }
                return eventBanners;
            });
        }

        public EventBanner GetEventBannerById(int eventBannerId)
        {
            if (eventBannerId == 0)
                return null;

            return _eventBannerRepository.GetById(eventBannerId);
        }

        public IList<EventBanner> GetEventBannersByIds(int[] eventBannerIds)
        {
            if (eventBannerIds == null || eventBannerIds.Length == 0)
                return new List<EventBanner>();

            var query = from c in _eventBannerRepository.Table
                where eventBannerIds.Contains(c.Id)
                select c;
            var eventBanners = query.ToList();
            //sort by passed identifiers
            var sortedEventBanners = new List<EventBanner>();
            foreach (int id in eventBannerIds)
            {
                var eventBanner = eventBanners.Find(x => x.Id == id);
                if (eventBanner != null)
                    sortedEventBanners.Add(eventBanner);
            }
            return sortedEventBanners;
        }

        public void InsertEventBanner(EventBanner eventBanner)
        {
            throw new NotImplementedException();
        }

        public void UpdateEventBanner(EventBanner eventBanner)
        {
            throw new NotImplementedException();
        }
    }
}
