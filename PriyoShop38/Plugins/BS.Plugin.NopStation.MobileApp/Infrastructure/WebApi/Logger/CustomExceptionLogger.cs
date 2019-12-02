using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace BS.Plugin.NopStation.MobileApp.Infrastructure.WebApi.Logger
{
    public class SimpleExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            if (context.Exception == null)
                return;
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //ignore 404 HTTP errors
            var httpException = context.Exception as HttpException;
            if (httpException != null && httpException.GetHttpCode() == 404 &&
                !EngineContext.Current.Resolve<CommonSettings>().Log404Errors)
                return;
            try
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(context.Exception.Message, context.Exception);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
    }
}
