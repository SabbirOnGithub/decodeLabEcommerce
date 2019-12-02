using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.FacebookExternalAuth
{
    public class FacebookExternalAuthModelApi
    {
        public string SystemName { get; set; }
        public string ProviderUserId { get; set; }
        public string AccessToken { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
