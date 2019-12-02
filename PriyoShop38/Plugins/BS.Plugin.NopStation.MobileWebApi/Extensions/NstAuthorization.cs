using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Nop.Core.Infrastructure;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;
using System.Web;
using Nop.Services.Configuration;
using BS.Plugin.NopStation.MobileWebApi.Models.NstSettingsModel;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System.Net.Http;
using System.Net;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public class NstAuthorization: ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            var identity = ParseNstAuthorizationHeader(filterContext);
            if (identity == false)
            {
                CreateNstAccessResponceMessage(filterContext);
                return;
            }
        }

        protected virtual bool ParseNstAuthorizationHeader(HttpActionContext actionContext)
        {
            var httpContext = EngineContext.Current.Resolve<HttpContextBase>();
            var keyFound = httpContext.Request.Headers.GetValues(Constant.NST);
            var requestkey = keyFound != null ? keyFound.FirstOrDefault() : "";
            try
            {
                var settingService = EngineContext.Current.Resolve<ISettingService>();
                var storeService = EngineContext.Current.Resolve<IStoreService>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                var storeScope = 0;
                if (storeService.GetAllStores().Count < 2)
                {
                    storeScope = 0;
                }
                else
                {
                    var storeId = workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
                    var store = storeService.GetStoreById(storeId);
                    storeScope = store?.Id ?? 0;
                }
                var nstSettings = settingService.LoadSetting<NstSettingsModel>(storeScope);

                var load = JWT.JsonWebToken.DecodeToObject(requestkey, nstSettings.NST_SECRET) as IDictionary<string, object>;
                if (load != null)
                {
                    return load[Constant.NST_KEY].ToString() == nstSettings.NST_KEY;
                }
            }
            catch
            {
                return false;
            }
            return false;

        }
        void CreateNstAccessResponceMessage(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            var response = new BaseResponse
            {
                StatusCode = (int)ErrorType.AuthenticationError,
                ErrorList = new List<string>
                {
                    "Nst Token Not Valid"
                }
            };
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, response);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
        }
    }
}
