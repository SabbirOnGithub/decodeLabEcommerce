using Nop.Core.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial class ContentManagementTemplateService : IContentManagementTemplateService
    {
        #region Fields

        private readonly IRepository<BS_ContentManagementTemplate> _topicTemplateRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="topicTemplateRepository">Topic template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ContentManagementTemplateService(IRepository<BS_ContentManagementTemplate> topicTemplateRepository,
            IEventPublisher eventPublisher)
        {
            this._topicTemplateRepository = topicTemplateRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void DeleteTopicTemplate(BS_ContentManagementTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Delete(topicTemplate);

            //event notification
            _eventPublisher.EntityDeleted(topicTemplate);
        }

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>Topic templates</returns>
        public virtual IList<BS_ContentManagementTemplate> GetAllTopicTemplates()
        {
            var query = from pt in _topicTemplateRepository.Table
                        orderby pt.DisplayOrder
                        select pt;

            var templates = query.ToList();
            return templates;
        }

        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>Topic template</returns>
        public virtual BS_ContentManagementTemplate GetTopicTemplateById(int topicTemplateId)
        {
            if (topicTemplateId == 0)
                return null;

            return _topicTemplateRepository.GetById(topicTemplateId);
        }

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void InsertTopicTemplate(BS_ContentManagementTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Insert(topicTemplate);

            //event notification
            _eventPublisher.EntityInserted(topicTemplate);
        }

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        public virtual void UpdateTopicTemplate(BS_ContentManagementTemplate topicTemplate)
        {
            if (topicTemplate == null)
                throw new ArgumentNullException("topicTemplate");

            _topicTemplateRepository.Update(topicTemplate);

            //event notification
            _eventPublisher.EntityUpdated(topicTemplate);
        }

        #endregion
    }
}
