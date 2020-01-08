
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.FacebookChat
{
    public class FacebookChatSettings : ISettings
    {
        public string FacebookChatPageId { get; set; }
        public string FacebookChatAppId { get; set; }
        public string ThemeColor { get; set; }
    }
}