using System.Collections.Generic;
using BS.Plugin.NopStation.MobileApp.Domain;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    /// <summary>
    /// Message template service
    /// </summary>
    public partial interface INotificationMessageTemplateService
    {
        /// <summary>
        /// Delete a message template
        /// </summary>
        /// <param name="notificationMessageTemplate">Message template</param>
        void DeleteNotificationMessageTemplate(NotificationMessageTemplate notificationMessageTemplate);

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="notificationMessageTemplate">Message template</param>
        void InsertNotificationMessageTemplate(NotificationMessageTemplate notificationMessageTemplate);

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="notificationMessageTemplate">Message template</param>
        void UpdateNotificationMessageTemplate(NotificationMessageTemplate notificationMessageTemplate);

        /// <summary>
        /// Gets a message template by identifier
        /// </summary>
        /// <param name="notificationMessageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        NotificationMessageTemplate GetNotificationMessageTemplateById(int notificationMessageTemplateId);

        /// <summary>
        /// Gets a message template by name
        /// </summary>
        /// <param name="notificationMessageTemplateName">Message template name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Message template</returns>
        NotificationMessageTemplate GetNotificationMessageTemplateByName(string notificationMessageTemplateName, int storeId);

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>Message template list</returns>
        IList<NotificationMessageTemplate> GetAllNotificationMessageTemplates(int storeId);

        /// <summary>
        /// Create a copy of message template with all depended data
        /// </summary>
        /// <param name="notificationMessageTemplate">Message template</param>
        /// <returns>Message template copy</returns>
        NotificationMessageTemplate CopyNotificationMessageTemplate(NotificationMessageTemplate notificationMessageTemplate);
    }
}
