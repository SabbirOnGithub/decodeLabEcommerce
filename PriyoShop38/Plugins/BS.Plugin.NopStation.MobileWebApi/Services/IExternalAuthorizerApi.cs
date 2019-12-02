using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Authentication.External;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public interface IExternalAuthorizerApi
    {
        AuthorizationResultApi Authorize(OpenAuthenticationParameters parameters);
    }
}
