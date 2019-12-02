using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileApp.Services;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Services
{
    public partial class InsertQueuedNotificationsTask : ITask
    {
       private readonly ILogger _logger;
        private readonly IQueuedNotificationService _queuedNotificationService;
        public InsertQueuedNotificationsTask(ILogger logger,
            IQueuedNotificationService queuedNotificationService)
        {
            this._logger = logger;
            this._queuedNotificationService = queuedNotificationService;
        }
        public void Execute()
        {
            try
            {
                _queuedNotificationService.InsertQueuedNoticationsAccordingToScheduleByTime(DateTime.UtcNow);
            }
            catch (Exception exc)
            {

                _logger.Error(string.Format("Error on inserting queued notification. {0}", exc.Message), exc);
            }
            
            
        }
    }
}
