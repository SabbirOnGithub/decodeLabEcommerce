using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.HomePageProduct.Task
{
    public partial class HomePageSaveTask : ITask
    {
        private readonly ILanguageService _languageService;
        private readonly IStoreContext _storeContext;

        public HomePageSaveTask(ILanguageService languageService, IStoreContext storeContext)
        {
            _languageService = languageService;
            _storeContext = storeContext;
        }
        public void Execute()
        {
            WebClient client = new WebClient();

            var languages = _languageService.GetAllLanguages();
            foreach (var lang in languages)
            {
                Stream data = client.OpenRead(_storeContext.CurrentStore.Url + "/home/index?lang=" + lang.Id);
                string s = null;
                if (data != null)
                {
                    StreamReader reader = new StreamReader(data);
                    s = reader.ReadToEnd();
                    data.Close();
                    reader.Close();
                }
                var filePath = CommonHelper.MapPath(string.Format("~/Plugins/Misc.HomePageProduct/special-category-{0}.txt", lang.Id));
                File.WriteAllText(filePath, String.Empty);
                File.WriteAllText(filePath, s);

                filePath = CommonHelper.MapPath(string.Format("~/Plugins/Misc.HomePageProduct/special-category-{0}-backup.txt", lang.Id));
                File.WriteAllText(filePath, String.Empty);
                File.WriteAllText(filePath, s);
            }
        }
    }
}