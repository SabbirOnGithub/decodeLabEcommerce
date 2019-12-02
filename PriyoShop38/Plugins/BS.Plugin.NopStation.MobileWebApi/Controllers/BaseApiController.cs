using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class BaseApiController : ApiController
    {

        /// <summary>
        /// Get active store scope (for multi-store configuration mode)
        /// </summary>
        /// <param name="storeService">Store service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Store ID; 0 if we are in a shared mode</returns>
        public virtual int GetActiveStoreScopeConfiguration(IStoreService storeService, IWorkContext workContext)
        {
            //ensure that we have 2 (or more) stores
            if (storeService.GetAllStores().Count < 2)
                return 0;


            var storeId = workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = storeService.GetStoreById(storeId);
            return store != null ? store.Id : 0;
        }


        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="exc">Exception</param>
        protected void LogException(Exception exc)
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var logger = EngineContext.Current.Resolve<ILogger>();

            var customer = workContext.CurrentCustomer;
            logger.Error(exc.Message, exc, customer);
        }

        public int GetCustomerIdFromHeader()
        {
            IEnumerable<string> headerValues;
            var secretKey = Constant.SecretKey;
            if (Request.Headers.TryGetValues(Constant.TokenName, out headerValues))
            {
                var token = headerValues.FirstOrDefault();
                var payload = JWT.JsonWebToken.DecodeToObject(token, secretKey) as IDictionary<string, object>;
                if (payload != null && int.TryParse(payload[Constant.CustomerIdName].ToString(), out int customerId))
                    return customerId;
            }
            return 0;
        }

        public string GetDeviceIdFromHeader()
        {
            if (Request.Headers.TryGetValues(Constant.DeviceIdName, out IEnumerable<string> headerValues) && headerValues.Count() > 0)
            {
                var device = headerValues.FirstOrDefault();
                if (device != null)
                    return device;
            }
            return string.Empty;
        }

        protected internal bool TryUpdateModel<TModel>(TModel model, string prefix) where TModel : class
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var bindingContext = new ModelBindingContext()
            {
                Model = model,
                ModelName = prefix,
                ModelState = ModelState,
            };
            return ModelState.IsValid;
        }
    }
}
