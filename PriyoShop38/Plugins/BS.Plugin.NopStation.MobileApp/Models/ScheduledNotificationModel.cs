using System.Web.Mvc;
using FluentValidation.Attributes;
using BS.Plugin.NopStation.MobileApp.Validators;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    [Validator(typeof(ScheduledNotificationValidator))]
    public class ScheduledNotificationModel : BaseNopEntityModel
    {
        public ScheduledNotificationModel()
        {
            AvailableMessageTemplates=new List<SelectListItem>();
        }
        
        #region extra fileds
        public SelectList AvailableGroups { get; set; }
        public IList<SelectListItem> AvailableMessageTemplates { get; set; }
        #endregion
        public int Priority { get; set; }
        /// <summary>
        /// Gets or sets the group
        /// </summary>
        public int GroupId { get; set; }

        public string GroupName { get; set; }
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
        public string NotificationType { get; set; }
        public int ItemId { get; set; }                
        [UIHint("Picture")]
        [NopResourceDisplayName("BS.Plugin.NopStation.MobileApp.Picture")]
        public int PictureId { get; set; }
    }
}
