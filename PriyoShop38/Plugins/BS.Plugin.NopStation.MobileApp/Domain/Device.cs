using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Domain
{
    public partial class Device : BaseEntity
    {

        public string DeviceToken { get; set; }
        public int DeviceTypeId { get; set; }
        public int CustomerId { get; set; }
        public string SubscriptionId { get; set; }
        public bool IsRegistered { get; set; }
        /// <summary>
        /// Gets or sets the date and time of Device creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of Device update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

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

    }
}
