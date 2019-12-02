using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class PushNotificationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.PushNotificationHeading")]
        public string PushNotificationHeading { get; set; }
        public bool PushNotificationHeading_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.PushNotificationMessage")]
        public string PushNotificationMessage { get; set; }
        public bool PushNotificationMessage_OverrideForStore { get; set; }

    }
    public class QueuedNotification : BaseNopModel
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
        public string SubscriptionId { get; set; }


        /// <summary>
        /// represent the group entity
        /// </summary>
        public int GroupId { get; set; }
        //public BsSmartGroup Group { get; set; }

        /// <summary>
        /// Gets or sets the send Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the send Body
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

        public DeviceType DeviceType
        {
            get
            {
                return (DeviceType)this.DeviceTypeId;
            }
            set
            {
                this.DeviceTypeId = (int)value;
            }
        }
        //public int NotificationTypeId { get; set; }
        //public NotificationType NotificationType
        //{
        //    get
        //    {
        //        return (NotificationType)this.NotificationTypeId;
        //    }
        //    set
        //    {
        //        this.NotificationTypeId = (int)value;
        //    }
        //}

        public int ItemId { get; set; }
    }

    public class CustomObjectModel
    {
        public int NotificationTypeId { get; set; }
        public string NotificationType { get; set; }
        public int ItemId { get; set; }

        public string Subject { get; set; }
    }
}