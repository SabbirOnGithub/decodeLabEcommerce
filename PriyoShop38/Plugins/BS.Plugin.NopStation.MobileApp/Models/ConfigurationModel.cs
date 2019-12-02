using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileApp.Models
{
   public class ConfigurationModel:BaseNopModel
   {
       #region ios
       public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("BS.Plugin.NopStation.MobileApp.AppleCertFileNameWithPath")]
       public string AppleCertFileNameWithPath { get; set; }
        public bool AppleCertFileNameWithPath_OverrideForStore { get; set; }

        [NopResourceDisplayName("BS.Plugin.NopStation.MobileApp.ApplePassword")]
        public string ApplePassword { get; set; }
        public bool ApplePassword_OverrideForStore { get; set; }

       [NopResourceDisplayName("BS.Plugin.NopStation.MobileApp.IsAppleProductionMode")]
        public bool IsAppleProductionMode { get; set; }


       #endregion
       #region andriod

        /// <summary>
        ///Android GCM Notification-- YOUR Google API's Console API Access  API KEY for Server Apps HERE
        /// </summary>
        [NopResourceDisplayName("BS.Plugin.NopStation.MobileApp.GoogleConsoleAPIAccess_KEY")]
        public string GoogleConsoleAPIAccess_KEY { get; set; }
        public bool GoogleConsoleAPIAccess_KEY_OverrideForStore { get; set; }

        [NopResourceDisplayName("BS.Plugin.NopStation.MobileApp.GoogleProject_Number")]
         public string GoogleProject_Number { get; set; }

       #endregion
   }
    
}
