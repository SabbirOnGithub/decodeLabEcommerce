using System.Net;
using Nop.Core;
using Nop.Services.Tasks;
using Nop.Services.Localization;
using System.IO;
using System;
using Nop.Services.Logging;

namespace Nop.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : ITask
    {
        private readonly IStoreContext _storeContext;
        private readonly ILanguageService _languageService;
        private readonly ILogger _logger;
        public KeepAliveTask(IStoreContext storeContext,
            ILanguageService languageService,
            ILogger logger)
        {
            this._storeContext = storeContext;
            this._languageService = languageService;
            this._logger = logger;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            string url = _storeContext.CurrentStore.Url + "/home/index?lang=";
            WebClient client = new WebClient();
            var languages = _languageService.GetAllLanguages();
            foreach (var lang in languages)
            {
                url = url + lang.Id;
                _logger.Debug(url);
                Stream data = client.OpenRead(url + lang.Id);
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
            //using (var wc = new WebClient())
            //{
            //    wc.DownloadString(url);
            //}
        }
    }
}
