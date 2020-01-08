using System;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class BrandWeekController : BasePublicController
    {
        public BrandWeekController() { }

        // GET: BrandWeek
        public ActionResult Index()
        {
            var model = new BrandWeekModel();
            model.StartDateTime = new DateTime(2019, 9, 15);
            model.CurrentDateTime = DateTime.UtcNow.AddHours(-6);
            return View(model);
        }
    }

    public class BrandWeekModel
    {
        public DateTime StartDateTime { get; set; }

        public DateTime CurrentDateTime { get; set; }
    }
}