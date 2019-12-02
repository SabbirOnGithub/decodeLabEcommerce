using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;

namespace BS.Plugin.NopStation.MobileApp.Domain
{
    /// <summary>
    /// Represent the scheduled notification
    /// Author: Md. Minul Islam Sohel, BrainStation-23 Ltd.
    /// </summary>
    public class ScheduledNotification:BaseEntity
    {
        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// Gets or sets the group
        /// </summary>
        public int GroupId { get; set; }
        //public BsSmartGroup Group { get; set; }

        /// <summary>
        /// Represents the notification subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Represents the notification body
        /// </summary>
        public string Message { get; set; }

        public int NotificationMessageTemplateId { get; set; }
       public DateTime CreatedOnUtc { get; set; }

        public DateTime SendingWillStartOnUtc { get; set; }

        public bool IsQueued { get; set; }
        public string ErrorLog { get; set; }
        public int QueuedCount { get; set; }

        public int NotificationTypeId { get; set; }

        public int ItemId { get; set; }                
        public int PictureId { get; set; }
        public NotificationType NotificationType
        {
            get
            {
                return (NotificationType)this.NotificationTypeId;
            }
            set
            {
                this.NotificationTypeId = (int)value;
            }
        }
    }
}
