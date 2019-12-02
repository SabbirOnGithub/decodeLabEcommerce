using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Common
{
    /// <summary>
    /// Represents a Event Banner
    /// </summary>
    public class EventBanner : BaseEntity, ILocalizedEntity
    {
        public int CategoryId { get; set; }
        public string BannerName { get; set; }
        public int PictureId { get; set; }
        public bool IsActive { get; set; }

        public virtual Category Category { get; set; }
    }
}
