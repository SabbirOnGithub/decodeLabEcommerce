using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.FacebookPixel.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.FacebookPixelScriptFirstPart")]
        [AllowHtml]
        public string FacebookPixelScriptFirstPart { get; set; }
        public bool FacebookPixelScriptFirstPart_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.FacebookPixel.FacebookPixelScriptLastPart")]
        [AllowHtml]
        public string FacebookPixelScriptLastPart { get; set; }
        public bool FacebookPixelScriptLastPart_OverrideForStore { get; set; }
       
        [AllowHtml]
        public string DefaultFacebookPixelScriptFirstPart { get; set; }
        [AllowHtml]
        public string DefaultFacebookPixelScriptLastPart { get; set; }





    }
}