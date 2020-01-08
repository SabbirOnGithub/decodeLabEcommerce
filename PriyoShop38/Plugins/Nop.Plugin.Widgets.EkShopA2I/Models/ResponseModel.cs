using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            meta = new MetaModel();
            response = new ResponseDetailsModel();
        }

        public MetaModel meta { get; set; }
        public ResponseDetailsModel response { get; set; } 
    }

    public class ResponseDetailsModel
    {
        public string message { get; set; }
        public string session_token { get; set; }
        public string redirect_url { get; set; }
        public int order_id { get; set; }
    }
}
