using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Nop.Plugin.Widgets.EkShopA2I.Extensions
{
    public class EkshopAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var _logger = EngineContext.Current.Resolve<ILogger>();
            var _ekshopSettings = EngineContext.Current.Resolve<EkshopSettings>();

            var identity = ParseAuthorizationHeader(actionContext, _logger, _ekshopSettings);
            if (identity == false)
            {
                Challenge(actionContext, _logger, _ekshopSettings);
                return;
            }
            base.OnAuthorization(actionContext);
        }

        protected virtual bool ParseAuthorizationHeader(HttpActionContext actionContext, ILogger logger, EkshopSettings ekshopSettings)
        {
            bool check = false;
            IEnumerable<string> checkApiKey;
            IEnumerable<string> checkAuthKey;
            if (actionContext.Request.Headers.TryGetValues(EkshopConstant.EsApiKey, out checkApiKey) &&
                actionContext.Request.Headers.TryGetValues(EkshopConstant.EsAuthorization, out checkAuthKey))
            {
                var apiKey = checkApiKey.FirstOrDefault();
                var authKey = checkAuthKey.FirstOrDefault();

                if (ekshopSettings.EnableLog)
                {
                    logger.Information("Ekshop Api Key (Ok): " + apiKey);
                    logger.Information("Ekshop Authorization (Ok): " + authKey);
                }

                try
                {
                    if (ekshopSettings.Authorization == authKey && ekshopSettings.ApiKey == apiKey)
                    {
                        check = true;
                    }
                }
                catch (Exception ex)
                {
                    check = false;
                    logger.Error(ex.Message, ex);
                }
            }
            else if (ekshopSettings.EnableLog)
            {
                if (actionContext.Request.Headers.TryGetValues(EkshopConstant.EsApiKey, out checkApiKey))
                {
                    var apiKey = checkApiKey.FirstOrDefault();
                    logger.Information("Ekshop Api Key (Not ok): " + apiKey);
                }
                else
                {
                    logger.Information("Ekshop Api Key (Not ok): null");
                }

                if (actionContext.Request.Headers.TryGetValues(EkshopConstant.EsAuthorization, out checkAuthKey))
                {
                    var authKey = checkAuthKey.FirstOrDefault();
                    logger.Information("Ekshop Authorization (Not ok): " + authKey);
                }
                else
                {
                    logger.Information("Ekshop Authorization (Not ok): null");
                }
            }
            return check;
        }

        void Challenge(HttpActionContext actionContext, ILogger logger, EkshopSettings ekshopSettings)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            if (ekshopSettings.EnableLog)
                logger.Information("Invaid authorization or api key");

            var response = new
            {
                meta = new { status = 100 },
                response = new { message = "Invaid authorization or api key" }
            };
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, response);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
        }
    }
}