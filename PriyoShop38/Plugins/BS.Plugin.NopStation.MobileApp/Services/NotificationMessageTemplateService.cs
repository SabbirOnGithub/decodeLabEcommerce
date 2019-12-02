using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using BS.Plugin.NopStation.MobileApp.Domain;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Stores;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    public partial class NotificationMessageTemplateService: INotificationMessageTemplateService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// </remarks>
        private const string NOTIFICATIONMESSAGETEMPLATES_ALL_KEY = "Nop.notificationmessagetemplate.all-{0}";
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : template name
        /// {1} : store ID
        /// </remarks>
        private const string NOTIFICATIONMESSAGETEMPLATES_BY_NAME_KEY = "Nop.notificationmessagetemplate.name-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string NOTIFICATIONMESSAGETEMPLATES_PATTERN_KEY = "Nop.notificationmessagetemplate.";

        #endregion

        #region Fields

        private readonly IRepository<NotificationMessageTemplate> _notificationMessageTemplateRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ILanguageService _languageService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="languageService">Language service</param>
        /// <param name="localizedEntityService">Localized entity service</param>
        /// <param name="storeMappingService">Store mapping service</param>
        /// <param name="notificationMessageTemplateRepository">Message template repository</param>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <param name="eventPublisher">Event published</param>
        public NotificationMessageTemplateService(ICacheManager cacheManager,
            IRepository<StoreMapping> storeMappingRepository,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IStoreMappingService storeMappingService,
            IRepository<NotificationMessageTemplate> notificationMessageTemplateRepository,
            CatalogSettings catalogSettings,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._storeMappingRepository = storeMappingRepository;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._storeMappingService = storeMappingService;
            this._notificationMessageTemplateRepository = notificationMessageTemplateRepository;
            this._catalogSettings = catalogSettings;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a message template
        /// </summary>
        /// <param name="notificationMessageTemplate">Message template</param>
        public virtual void DeleteNotificationMessageTemplate(NotificationMessageTemplate notificationMessageTemplate)
        {
            if (notificationMessageTemplate == null)
                throw new ArgumentNullException("notificationMessageTemplate");

            _notificationMessageTemplateRepository.Delete(notificationMessageTemplate);

            _cacheManager.RemoveByPattern(NOTIFICATIONMESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(notificationMessageTemplate);
        }

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="notificationMessageTemplate">Message template</param>
        public virtual void InsertNotificationMessageTemplate(NotificationMessageTemplate notificationMessageTemplate)
        {
            if (notificationMessageTemplate == null)
                throw new ArgumentNullException("notificationMessageTemplate");

            _notificationMessageTemplateRepository.Insert(notificationMessageTemplate);

            _cacheManager.RemoveByPattern(NOTIFICATIONMESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(notificationMessageTemplate);
        }

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="notificationMessageTemplate">Message template</param>
        public virtual void UpdateNotificationMessageTemplate(NotificationMessageTemplate notificationMessageTemplate)
        {
            if (notificationMessageTemplate == null)
                throw new ArgumentNullException("notificationMessageTemplate");

            _notificationMessageTemplateRepository.Update(notificationMessageTemplate);

            _cacheManager.RemoveByPattern(NOTIFICATIONMESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(notificationMessageTemplate);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="notificationMessageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        public virtual NotificationMessageTemplate GetNotificationMessageTemplateById(int notificationMessageTemplateId)
        {
            if (notificationMessageTemplateId == 0)
                return null;

            return _notificationMessageTemplateRepository.GetById(notificationMessageTemplateId);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="notificationMessageTemplateName">Message template name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Message template</returns>
        public virtual NotificationMessageTemplate GetNotificationMessageTemplateByName(string notificationMessageTemplateName, int storeId)
        {
            if (string.IsNullOrWhiteSpace(notificationMessageTemplateName))
                throw new ArgumentException("notificationMessageTemplateName");

            string key = string.Format(NOTIFICATIONMESSAGETEMPLATES_BY_NAME_KEY, notificationMessageTemplateName, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = _notificationMessageTemplateRepository.Table;
                query = query.Where(t => t.Name == notificationMessageTemplateName);
                query = query.OrderBy(t => t.Id);
                query = query.OrderBy(t => t.Id);
                var templates = query.ToList();

                //store mapping
                if (storeId > 0)
                {
                    templates = templates
                        .Where(t => _storeMappingService.Authorize(t, storeId))
                        .ToList();
                }

                return templates.FirstOrDefault();
            });

        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>Message template list</returns>
        public virtual IList<NotificationMessageTemplate> GetAllNotificationMessageTemplates(int storeId)
        {
            string key = string.Format(NOTIFICATIONMESSAGETEMPLATES_ALL_KEY, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = _notificationMessageTemplateRepository.Table;
                query = query.OrderBy(t => t.Name);

                //Store mapping
                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    query = from t in query
                            join sm in _storeMappingRepository.Table
                            on new { c1 = t.Id, c2 = "NotificationMessageTemplate" } equals new { c1 = sm.EntityId, c2 = sm.EntityName } into t_sm
                            from sm in t_sm.DefaultIfEmpty()
                            where !t.LimitedToStores || storeId == sm.StoreId
                            select t;

                    //only distinct items (group by ID)
                    query = from t in query
                            group t by t.Id
                            into tGroup
                            orderby tGroup.Key
                            select tGroup.FirstOrDefault();
                    query = query.OrderBy(t => t.Name);
                }

                return query.ToList();
            });
        }

        /// <summary>
        /// Create a copy of message template with all depended data
        /// </summary>
        /// <param name="notificationMessageTemplate">Message template</param>
        /// <returns>Message template copy</returns>
        public virtual NotificationMessageTemplate CopyNotificationMessageTemplate(NotificationMessageTemplate notificationMessageTemplate)
        {
            if (notificationMessageTemplate == null)
                throw new ArgumentNullException("notificationMessageTemplate");

            var mtCopy = new NotificationMessageTemplate
            {
                Name = notificationMessageTemplate.Name,
                Subject = notificationMessageTemplate.Subject,
                Body = notificationMessageTemplate.Body,
                IsActive = notificationMessageTemplate.IsActive,
                AttachedDownloadId = notificationMessageTemplate.AttachedDownloadId,
               LimitedToStores = notificationMessageTemplate.LimitedToStores,
            };

            InsertNotificationMessageTemplate(mtCopy);

            var languages = _languageService.GetAllLanguages(true);

            //localization
            foreach (var lang in languages)
            {
                
                var subject = notificationMessageTemplate.GetLocalized(x => x.Subject, lang.Id, false, false);
                if (!String.IsNullOrEmpty(subject))
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.Subject, subject, lang.Id);

                var body = notificationMessageTemplate.GetLocalized(x => x.Body, lang.Id, false, false);
                if (!String.IsNullOrEmpty(body))
                    _localizedEntityService.SaveLocalizedValue(mtCopy, x => x.Body, body, lang.Id);

            }

            //store mapping
            var selectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(notificationMessageTemplate);
            foreach (var id in selectedStoreIds)
            {
                _storeMappingService.InsertStoreMapping(mtCopy, id);
            }

            return mtCopy;
        }

        #endregion
    }
}
