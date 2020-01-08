using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    public partial class QueuedNotificationSendTask : ITask
    {
        private readonly ILogger _logger;
        private readonly IQueuedNotificationService _queuedNotificationService;
        public QueuedNotificationSendTask(ILogger logger,
            IQueuedNotificationService queuedNotificationService)
        {
            this._logger = logger;
            this._queuedNotificationService = queuedNotificationService;
        }
        public void Execute()
        {
            try
            {
                _queuedNotificationService.SendAllUnSendQueuedNotication(maxTries: 3);
            }
            catch (Exception exc)
            {

                _logger.Error(string.Format("Error sending notification. {0}", exc.Message), exc);
            }

            
            
        }


        public int Order
        {
            //ensure that this task is run first 
            get { return 1; }
        }
    }
}
