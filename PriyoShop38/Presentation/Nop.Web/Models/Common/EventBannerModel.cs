using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Common
{
    [Validator(typeof(EventBannerValidator))]
    public partial class EventBannerModel : BaseNopEntityModel
    {

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string BannerName { get; set; }
        public int PictureId { get; set; }
        public bool IsActive { get; set; }
        public string PictureUrl { get; set; }
    }
}