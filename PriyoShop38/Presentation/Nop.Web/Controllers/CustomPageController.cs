using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class CustomPageController : BasePublicController
    {
        // GET: CustomPage
        public ActionResult Index()
        {
            return View();
        }
    }
}