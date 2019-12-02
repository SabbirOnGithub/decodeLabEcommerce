using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;

namespace Nop.Services.Common
{
    public partial interface IEventBannerService
    {
        /// <summary>
        /// Deletes a eventBanner
        /// </summary>
        /// <param name="eventBanner">EventBanner</param>
        void DeleteEventBanner(EventBanner eventBanner);

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="languageId">Language identifier. It's used to sort countries by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Countries</returns>
        IList<EventBanner> getAllEventBanners(int languageId = 0, bool showHidden = false);


        /// <summary>
        /// Gets a eventBanner 
        /// </summary>
        /// <param name="eventBannerId">EventBanner identifier</param>
        /// <returns>EventBanner</returns>
        EventBanner GetEventBannerById(int eventBannerId);

        /// <summary>
        /// Get countries by identifiers
        /// </summary>
        /// <param name="eventBannerIds">EventBanner identifiers</param>
        /// <returns>Countries</returns>
        IList<EventBanner> GetEventBannersByIds(int[] eventBannerIds);


        /// <summary>
        /// Inserts a eventBanner
        /// </summary>
        /// <param name="eventBanner">EventBanner</param>
        void InsertEventBanner(EventBanner eventBanner);

        /// <summary>
        /// Updates the eventBanner
        /// </summary>
        /// <param name="eventBanner">EventBanner</param>
        void UpdateEventBanner(EventBanner eventBanner);
    }
}
