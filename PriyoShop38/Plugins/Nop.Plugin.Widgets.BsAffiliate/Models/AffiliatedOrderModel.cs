using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsAffiliate.Models
{
    public class AffiliatedOrderModel
    {
        public AffiliatedOrderModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderId")]
        public int OrderId { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.CommissionPaymentStatus")]
        public CommissionPaymentStatus CommissionPaymentStatus { get; set; }
        public string CommissionPaymentStatusString { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.FirstName")]
        public string AffiliateFirstName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.LastName")]
        public string AffiliateLastName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.Email")]
        public string AffiliateEmail { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StoreId")]
        public int StoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderTotal")]
        public decimal OrderTotal { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.MarkedAsPaidOn")]
        public DateTime? MarkedAsPaidOn { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderPaymentStatus")]
        public PaymentStatus OrderPaymentStatus { get; set; }

        public string OrderPaymentStatusString { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderStatus")]
        public OrderStatus OrderStatus { get; set; }

        public string OrderStatusString { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderDate")]
        public DateTime OrderDate { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateName")]
        public string AffiliateName { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateCommission")]
        public decimal AffiliateCommission { get; set; }
        public string AffiliateCommissionString { get; set; }

        public int Id { get; set; }
        public string OrderTotalString { get; set; }
        public string OrderDateString { get; set; }
        public string MarkedAsPaidOnString { get; set; }
        public int AffiliateId { get; set; }
    }
}
