using Nop.Web.Framework;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.FacebookChat.Models
{
    public class FacebookChatModel
    {
        [NopResourceDisplayName("Plugins.Widgets.FacebookChat.FacebookChatPageId")]
        [Required]
        public string FacebookChatPageId { get; set; }
        public bool FacebookChatPageId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookChat.FacebookChatAppId")]
        [Required]
        public string FacebookChatAppId { get; set; }
        public bool FacebookChatAppId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.FacebookChat.ThemeColor")]
        public string ThemeColor { get; set; }
        public bool ThemeColor_OverrideForStore { get; set; }
    }
}
