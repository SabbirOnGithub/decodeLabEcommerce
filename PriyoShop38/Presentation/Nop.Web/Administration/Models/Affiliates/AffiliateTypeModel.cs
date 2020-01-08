using FluentValidation.Attributes;
using Nop.Admin.Validators.Affiliates;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Admin.Models.Affiliates
{
    [Validator(typeof(AffiliateTypeValidator))]
    public class AffiliateTypeModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.AffiliateTypes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.AffiliateTypes.Fields.NameUrlParameter")]
        public string NameUrlParameter { get; set; }

        [NopResourceDisplayName("Admin.AffiliateTypes.Fields.IdUrlParameter")]
        public string IdUrlParameter { get; set; }

        [NopResourceDisplayName("Admin.AffiliateTypes.Fields.Active")]
        public bool Active { get; set; }
    }
}