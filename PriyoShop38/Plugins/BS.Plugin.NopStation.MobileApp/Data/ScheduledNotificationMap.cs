using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileApp.Domain;
using Nop.Data.Mapping;

namespace BS.Plugin.NopStation.MobileApp.Data
{
    public class ScheduledNotificationMap : NopEntityTypeConfiguration<ScheduledNotification>
    {
        public ScheduledNotificationMap()
        {
            this.ToTable("Bs_ScheduledNotification");
            this.HasKey(x => x.Id);
            this.Ignore(x => x.NotificationType);

            //this.HasOptional(x => x.Group)
            //    .WithMany()
            //    .HasForeignKey(x => x.GroupId).WillCascadeOnDelete(false);
        }
    }
}
