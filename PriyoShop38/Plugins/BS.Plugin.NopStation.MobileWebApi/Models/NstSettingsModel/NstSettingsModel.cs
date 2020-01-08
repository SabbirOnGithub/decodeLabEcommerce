using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace BS.Plugin.NopStation.MobileWebApi.Models.NstSettingsModel
{
    public class NstSettingsModel: ISettings
    {
        public string NST_KEY { get; set; }
        public string NST_SECRET { get; set; }
    }
}
