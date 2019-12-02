using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public class TokenAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var identity = ParseAuthorizationHeader(actionContext);
            if (identity == false)
            {
                Challenge(actionContext);
                return;
            }
            base.OnAuthorization(actionContext);
        }

        protected virtual bool ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            bool check = true;
            IEnumerable<string> checkToken;
            if (actionContext.Request.Headers.TryGetValues(Constant.TokenName, out checkToken))
            {
                var token = checkToken.FirstOrDefault();
                var secretKey = Constant.SecretKey;
                try
                {
                    var payload = JWT.JsonWebToken.DecodeToObject(token, secretKey) as IDictionary<string, object>;
                    check = true;
                }
                catch
                {
                    check = false;
                }
            }
            return check;
        }

        void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            var response = new BaseResponse
            {
                StatusCode = (int)ErrorType.AuthenticationError,
                ErrorList = new List<string>
                {
                    "Token Expired.Please Login Again"
                }
            };
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, response);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
        }
    }
}