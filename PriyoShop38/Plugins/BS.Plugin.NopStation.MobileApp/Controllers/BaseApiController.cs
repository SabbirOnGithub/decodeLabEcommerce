using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BS.Plugin.NopStation.MobileApp.Controllers
{
    public class BaseApiController : ApiController
    {
        public string GetHeaderValue()
        {
            IEnumerable<string> headerValues;
            var keyFound = Request.Headers.TryGetValues("UserId", out headerValues);
            if (keyFound)
            {
                return headerValues.FirstOrDefault();
            }
            else return null;
        }
    }
}
