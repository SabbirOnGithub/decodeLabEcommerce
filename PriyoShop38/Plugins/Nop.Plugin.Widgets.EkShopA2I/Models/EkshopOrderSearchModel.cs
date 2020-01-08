using Nop.Web.Framework;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class EkshopOrderSearchModel
    {
        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.OrderSearchModel.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.OrderSearchModel.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.OrderSearchModel.OrderCode")]
        public string OrderCode { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.OrderSearchModel.LpContactNumber")]
        public string LpContactNumber { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.EkShopA2I.OrderSearchModel.LpCode")]
        public string LpCode { get; set; }
    }
}
