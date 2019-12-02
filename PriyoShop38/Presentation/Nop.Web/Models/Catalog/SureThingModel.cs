using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Catalog
{
    public class SureThingModel : BaseNopModel
    {
        public DateTime? StartDateTime { get; set; }

        public DateTime CurrentDateTime { get; set; }
    }
}