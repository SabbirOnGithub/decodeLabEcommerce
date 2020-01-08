using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class KaziNKaziTeaController : BasePublicController
    {
        public KaziNKaziTeaController() { }

        // GET: KaziNKaziTea
        public ActionResult Index()
        {
            return View();
        }
    }
}