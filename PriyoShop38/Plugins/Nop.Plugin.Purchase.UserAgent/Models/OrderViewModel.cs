using Nop.Plugin.Purchase.UserAgent.Services;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Purchase.UserAgent.Models
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            AvailableUserAgentTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Purchase.UserAgent.OrderId")]
        public int OrderId { get; set; }
        [NopResourceDisplayName("Admin.Purchase.UserAgent.UserAgent")]
        public string UserAgent { get; set; }
        [NopResourceDisplayName("Admin.Purchase.UserAgent.CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }
        [NopResourceDisplayName("Admin.Purchase.UserAgent.CustomerName")]
        public string CustomerName { get; set; }
        [NopResourceDisplayName("Admin.Purchase.UserAgent.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Purchase.UserAgent.UserAgent")]
        public int UserAgentId { get; set; }

        public List<SelectListItem> AvailableUserAgentTypes { get; set; }
    }
}
