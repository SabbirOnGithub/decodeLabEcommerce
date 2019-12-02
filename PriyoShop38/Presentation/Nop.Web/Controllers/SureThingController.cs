using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Catalog;

using System;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class SureThingController : BasePublicController
    {
        private readonly CatalogSettings _catalogSettings;

        public SureThingController(CatalogSettings catalogSettings)
        {
            _catalogSettings = catalogSettings;
        }

        // GET: SureThing
        public ActionResult Index()
        {
            if (!_catalogSettings.EnableSureThing)
            {
                return RedirectToRoute("HomePage");
            }

            var model = new SureThingModel
            {
                StartDateTime = _catalogSettings.SureThingStartTime,
                CurrentDateTime = DateTime.UtcNow.AddHours(-6)
            };

            return View(model);
        }
    }
}