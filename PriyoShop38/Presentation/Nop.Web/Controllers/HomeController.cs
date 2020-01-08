using System.Web.Mvc;
using Nop.Web.Framework.Security;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Web.Models.Home;
using System.IO;
using Nop.Core.Caching;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ICacheManager _cacheManager;


        public HomeController(IWorkContext workContext,
            ILanguageService languageService,
            ICacheManager cacheManager)
        {
            this._workContext = workContext;
            this._languageService = languageService;
            this._cacheManager = cacheManager;
        }

        #region brainstation-23
        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult Index(int lang = 0)
        {
            var model = new HomeModel();
            if (lang == 0)
            {
                var cacheKey = string.Format("HomePageModel.SpecialCategory");

                model.Html = _cacheManager.Get(cacheKey, () =>
                {
                    var filePath = CommonHelper.MapPath(string.Format("~/Plugins/Misc.HomePageProduct/special-category-{0}.txt", _workContext.WorkingLanguage.Id));
                    var file = new FileInfo(filePath);
                    var jsonSpecialCategory = string.Empty;
                    if ((file.Attributes & FileAttributes.ReadOnly) != 0)
                    {
                        jsonSpecialCategory = System.IO.File.ReadAllText(filePath);
                    }
                    else
                    {
                        filePath = CommonHelper.MapPath(string.Format("~/Plugins/Misc.HomePageProduct/special-category-{0}-backup.txt", _workContext.WorkingLanguage.Id));
                        jsonSpecialCategory = System.IO.File.ReadAllText(filePath);
                    }
                    return jsonSpecialCategory;
                });
                
               
                return View("~/Themes/Pavilion/Views/Home/IndexLayout.cshtml",model);
            }
            var language = _languageService.GetLanguageById(lang);
            _workContext.WorkingLanguage = language;
            return View("~/Themes/Pavilion/Views/Home/IndexBody.cshtml");
        }

        public ActionResult SecondIndex(int lang)
        {
            var language= _languageService.GetLanguageById(lang);
            _workContext.WorkingLanguage = language;
            return View("~/Themes/Pavilion/Views/Home/Index.cshtml");
        }

        #endregion
    }
}
