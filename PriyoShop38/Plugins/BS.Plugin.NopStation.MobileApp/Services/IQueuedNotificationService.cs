using Nop.Core;
using BS.Plugin.NopStation.MobileApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Services
{
   public interface IQueuedNotificationService
    {
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all QueuedNotification.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<QueuedNotification> GetAllQueuedNotification(int pageIndex, int pageSize);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all email .
        /// </summary>
        /// <returns></returns>
        List<QueuedNotification> GetAllQueuedNotification();

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts the QueuedNotification.
        /// </summary>
        /// <param name="smartGroup">The sQueuedNotification.</param>
        QueuedNotification InsertQueuedNotification(QueuedNotification notification);


       ///--------------------------------------------------------------------------------------------
       /// <summary>
       /// Inserts the QueuedNotification.
       /// </summary>
        /// <param name="time">The sQueuedNotification.</param>
       void InsertQueuedNoticationsAccordingToScheduleByTime(DateTime time);
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the smart group by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        QueuedNotification GetQueuedNotificationById(int id);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the smart group.
        /// </summary>
        /// <param name="notification">The smart group.</param>
        void UpdateQueuedNotification(QueuedNotification notification);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the QueuedNotification.
        /// </summary>
        /// <param name="notification">The QueuedNotification.</param>
        void DeleteQueuedNotification(QueuedNotification notification);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Sends the IOS and Android notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void SendQueuedNotication(QueuedNotification notification);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Send all un send the IOS and Android notification.
        /// </summary>
        /// <param name="maxTries">The maximum number of tries.</param>
        void SendAllUnSendQueuedNotication(int  maxTries=5);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Test Sends the IOS and Android notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void SendTestNotication(QueuedNotification notification);

    }
}
