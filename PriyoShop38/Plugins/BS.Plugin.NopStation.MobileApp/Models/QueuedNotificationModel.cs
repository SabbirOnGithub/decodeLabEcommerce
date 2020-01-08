using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Attributes;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.Validators;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    [Validator(typeof(QueuedNotificationValidator))]
    public class QueuedNotificationModel:BaseNopEntityModel
    {
        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the ToCustomer property
        /// </summary>
        public int ToCustomerId { get; set; }

        public int DeviceTypeId { get; set; }

        public string DeviceType { get; set; }
        public string SubscriptionId { get; set; }

        public string ToCustomerName { get; set; }
        /// <summary>
        /// represent the group entity
        /// </summary>
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the send subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the send Message
        /// </summary>
        public string Message { get; set; }
       
       
        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        public DateTime? SentOnUtc { get; set; }
        /// <summary>
        /// represent notification is sent or not
        /// </summary>
        public bool IsSent { get; set; }

        /// <summary>
        /// represent notification sending error
        /// </summary>
        public string ErrorLog { get; set; }
        public int NotificationTypeId { get; set; }
        public string NotificationType { get; set; }
        public int ItemId { get; set; }
        public string Image { get; set; }

    }
}
