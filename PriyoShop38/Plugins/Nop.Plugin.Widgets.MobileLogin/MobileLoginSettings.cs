using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.MobileLogin
{
    public class MobileLoginSettings : ISettings
    {
        public string CountryCode { get; set; }
        public string SmsGatewayUserName { get; set; }
        public string SmsGatewayPassword { get; set; }
    }
}