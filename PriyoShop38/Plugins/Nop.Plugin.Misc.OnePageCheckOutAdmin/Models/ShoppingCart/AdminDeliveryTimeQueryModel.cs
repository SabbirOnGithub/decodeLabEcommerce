using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.ShoppingCart
{
    public class AdminDeliveryTimeQueryModel 
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int DeliveryTimeId { get; set; }
        public string CustomerEmail { get; set; }
    }
}
