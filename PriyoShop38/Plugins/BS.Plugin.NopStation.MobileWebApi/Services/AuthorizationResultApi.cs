using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Authentication.External;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class AuthorizationResultApi : AuthorizationResult
    {
        public AuthorizationResultApi(OpenAuthenticationStatus status, int customerId) : base(status)
        {
            this.CustomerId = customerId;
        }

        public int CustomerId { get; set; }
    }
}
