using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileApp.Data;
using BS.Plugin.NopStation.MobileApp.Domain;
using Nop.Core.Data;
using Nop.Services.Events;
using Nop.Core.Domain.Common;
using Nop.Core;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    /// <summary>
    /// Service for scheduled notification
    /// </summary>
   public class ScheduledNotificationService:IScheduledNotificationService
   {
       #region fields

       private readonly IRepository<ScheduledNotification> _scheduleRepository;
       private readonly IRepository<QueuedNotification> _queuedNotificationRepository;
       private readonly IRepository<Device> _deviceRepository; 
       private readonly IEventPublisher _eventPublisher;
       private readonly MobileAppObjectContext _dbContext;
       private readonly CommonSettings _commonSettings;
      

       #endregion
       #region ctor
       public ScheduledNotificationService(
           IRepository<ScheduledNotification> scheduleRepository,
           IEventPublisher eventPublisher,
           MobileAppObjectContext dbContext,
           CommonSettings commonSettings,
           IRepository<QueuedNotification> queuedNotificationRepository,
           IRepository<Device> deviceRepository
           )
       {
           this._scheduleRepository = scheduleRepository;
           this._eventPublisher = eventPublisher;
           this._dbContext = dbContext;
           this._commonSettings = commonSettings;
           this._queuedNotificationRepository = queuedNotificationRepository;
           this._deviceRepository = deviceRepository;
       }
       #endregion

       #region Implementation of IScheduleService

      

     
       ///--------------------------------------------------------------------------------------------
       /// <summary>
       /// Gets all schedule.
       /// </summary>
       /// <param name="pageIndex">Index of the page.</param>
       /// <param name="pageSize">Size of the page.</param>
       /// <returns></returns>
       public virtual IPagedList<ScheduledNotification> GetAllSchedule(int pageIndex, int pageSize)
       {
           var query = (from s in _scheduleRepository.Table
                        orderby s.SendingWillStartOnUtc
                        select s);
           var schedules = new PagedList<ScheduledNotification>(query, pageIndex, pageSize);
           return schedules;
       }

       ///--------------------------------------------------------------------------------------------
       /// <summary>
       /// Gets all schedule by time.
       /// </summary>
       /// <param name="time">The time.</param>
       /// <returns></returns>
       public virtual IEnumerable<ScheduledNotification> GetAllScheduleByTime(DateTime time)
       {
           return
               from s in _scheduleRepository.Table
               where s.SendingWillStartOnUtc < time && s.IsQueued.Equals(false)
               select s;
       }

       ///--------------------------------------------------------------------------------------------
       /// <summary>
       /// Inserts the schedule.
       /// </summary>
       /// <param name="schedule">The schedule.</param>
       /// <returns></returns>
       public virtual ScheduledNotification InsertSchedule(ScheduledNotification schedule)
       {
           if (schedule == null)
               throw new ArgumentNullException("schedule");

           _scheduleRepository.Insert(schedule);
           //event notification
           _eventPublisher.EntityInserted(schedule);

           return schedule;
       }

       ///--------------------------------------------------------------------------------------------
       /// <summary>
       /// Gets the schedule by id.
       /// </summary>
       /// <param name="id">The id.</param>
       /// <returns></returns>
       public ScheduledNotification GetScheduleById(int id)
       {
           var query = _scheduleRepository;
           return query.GetById(id);
       }



       ///--------------------------------------------------------------------------------------------
       /// <summary>
       /// Updates the schedule.
       /// </summary>
       /// <param name="schedule">The schedule.</param>
       public void UpdateSchedule(ScheduledNotification schedule)
       {
           if (schedule == null)
               throw new ArgumentNullException("schedule");

           _scheduleRepository.Update(schedule);
           //event notification
           _eventPublisher.EntityUpdated(schedule);

       }

       ///--------------------------------------------------------------------------------------------
       /// <summary>
       /// Deletes the schedule.
       /// </summary>
       /// <param name="schedule">The schedule.</param>
       public void DeleteSchedule(ScheduledNotification schedule)
       {
           _scheduleRepository.Delete(schedule);
           _eventPublisher.EntityDeleted(schedule);
       }

      
       #endregion


   }
}
