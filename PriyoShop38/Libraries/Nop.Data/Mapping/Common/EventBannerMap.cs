using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    class EventBannerMap : NopEntityTypeConfiguration<EventBanner>
    {
        public EventBannerMap()
        {
            this.ToTable("EventBanner");
            this.HasKey(c => c.Id);
            this.Property(c => c.BannerName).IsRequired().HasMaxLength(150);
            this.Property(c => c.IsActive).IsRequired();
            this.Property(c => c.CategoryId).IsRequired();
        }
    }
}
