using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Plugin.Widgets.BsMegaMenu
{
    public class MegaMenuScheduleTask : ITask
    {
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        public MegaMenuScheduleTask(ILogger logger, IStoreContext storeContext)
        {
            this._logger = logger;
            this._storeContext = storeContext;
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Execute task
        /// </summary>
        public void Execute()
        {
            string url = _storeContext.CurrentStore.Url + "bsmegamenucache";
            using (var wc = new WebClient())
            {
                wc.DownloadString(url);
            }
        }



        public int Order
        {
            //ensure that this task is run first 
            get { return 0; }
        }
    }
    
}
