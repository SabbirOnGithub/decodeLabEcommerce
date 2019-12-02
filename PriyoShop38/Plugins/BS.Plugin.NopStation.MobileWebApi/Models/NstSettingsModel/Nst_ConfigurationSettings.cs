using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.NstSettingsModel
{
   public class Nst_ConfigurationSettings : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_KEY")]
        public string NST_KEY { get; set; }
        public bool NST_KEY_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_SECRET")]
        public string NST_SECRET { get; set; }
        public bool NST_SECRET_OverrideForStore { get; set; }

        public string NST { get; set; }
        public bool EditMode { get; set; }
    }
}
