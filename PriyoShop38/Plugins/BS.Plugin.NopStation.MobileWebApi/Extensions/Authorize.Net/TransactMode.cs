using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions.Authorize.Net
{
   
        public enum TransactMode
        {
            /// <summary>
            /// Authorize
            /// </summary>
            Authorize = 1,
            /// <summary>
            /// Authorize and capture
            /// </summary>
            AuthorizeAndCapture = 2
        }
    
}
