using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public partial class CustomJsonResult
    {
        public string redirect { get; set; }

        public bool success { get; set; }

        public string message { get; set; }

    }
}
