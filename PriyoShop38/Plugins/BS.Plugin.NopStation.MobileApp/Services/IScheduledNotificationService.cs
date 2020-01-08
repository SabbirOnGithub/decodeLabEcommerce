using Nop.Core;
using BS.Plugin.NopStation.MobileApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Services
{
   public interface IScheduledNotificationService
    {
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all schedule.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<ScheduledNotification> GetAllSchedule(int pageIndex, int pageSize);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all schedule by time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        IEnumerable<ScheduledNotification> GetAllScheduleByTime(DateTime time);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts the schedule.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <returns></returns>
        ScheduledNotification InsertSchedule(ScheduledNotification schedule);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the schedule by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        ScheduledNotification GetScheduleById(int id);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the schedule.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        void UpdateSchedule(ScheduledNotification schedule);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the schedule.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        void DeleteSchedule(ScheduledNotification schedule);

        
        
    }
}
