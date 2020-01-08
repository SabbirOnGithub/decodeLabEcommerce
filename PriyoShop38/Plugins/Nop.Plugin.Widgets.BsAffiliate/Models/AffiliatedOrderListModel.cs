using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    public class AffiliatedOrderListModel
    {
        public AffiliatedOrderListModel()
        {
            AvailableOrderStatuses = new List<SelectListItem>();
            AvailablePaymentStatuses = new List<SelectListItem>();
            AvailableShippingStatuses = new List<SelectListItem>();
        }

        public int AffliateId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.Orders.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.Orders.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.Orders.OrderStatus")]
        public int OrderStatusId { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.Orders.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.Orders.ShippingStatus")]
        public int ShippingStatusId { get; set; }

        public IList<SelectListItem> AvailableOrderStatuses { get; set; }
        public IList<SelectListItem> AvailablePaymentStatuses { get; set; }
        public IList<SelectListItem> AvailableShippingStatuses { get; set; }
    }
}
