using BS.Plugin.NopStation.MobileWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    /// <summary>
    /// Topic template service interface
    /// </summary>
    public partial interface IContentManagementTemplateService
    {
        /// <summary>
        /// Delete topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        void DeleteTopicTemplate(BS_ContentManagementTemplate topicTemplate);

        /// <summary>
        /// Gets all topic templates
        /// </summary>
        /// <returns>Topic templates</returns>
        IList<BS_ContentManagementTemplate> GetAllTopicTemplates();

        /// <summary>
        /// Gets a topic template
        /// </summary>
        /// <param name="topicTemplateId">Topic template identifier</param>
        /// <returns>Topic template</returns>
        BS_ContentManagementTemplate GetTopicTemplateById(int topicTemplateId);

        /// <summary>
        /// Inserts topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        void InsertTopicTemplate(BS_ContentManagementTemplate topicTemplate);

        /// <summary>
        /// Updates the topic template
        /// </summary>
        /// <param name="topicTemplate">Topic template</param>
        void UpdateTopicTemplate(BS_ContentManagementTemplate topicTemplate);
    }
}
