using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Common
{
    public class EventBannerModel : BaseNopEntityModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BannerName { get; set; }
        public int PictureId { get; set; }
        public bool IsActive { get; set; }

    }
}