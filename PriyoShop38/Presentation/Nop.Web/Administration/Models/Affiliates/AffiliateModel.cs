using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Affiliates
{
    public partial class AffiliateModel : BaseNopEntityModel
    {
        public AffiliateModel()
        {
            Address = new AddressModel();
            AvailableAffiliateTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Affiliates.Fields.ID")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.URL")]
        public string Url { get; set; }


        [NopResourceDisplayName("Admin.Affiliates.Fields.AdminComment")]
        [AllowHtml]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.FriendlyUrlName")]
        [AllowHtml]
        public string FriendlyUrlName { get; set; }
        
        [NopResourceDisplayName("Admin.Affiliates.Fields.Active")]
        public bool Active { get; set; }

        public AddressModel Address { get; set; }

        #region BS-23

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public int AffiliateTypeId { get; set; }

        public IList<SelectListItem> AvailableAffiliateTypes { get; set; }

        [NopResourceDisplayName("Admin.Affiliates.Fields.BKashNumber")]
        public string BKashNumber { get; set; }

        #endregion


        #region Nested classes

        public partial class AffiliatedOrderModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Affiliates.Orders.Order")]
            public override int Id { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderStatus")]
            public string OrderStatus { get; set; }
            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderStatus")]
            public int OrderStatusId { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.PaymentStatus")]
            public string PaymentStatus { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.ShippingStatus")]
            public string ShippingStatus { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.OrderTotal")]
            public string OrderTotal { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.CreatedOn")]
            public DateTime CreatedOn { get; set; }


            #region BS-23

            [NopResourceDisplayName("Admin.Affiliates.Orders.AffiliateCommission")]
            public string AffiliateCommission { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.AffiliateCommission")]
            public decimal AffiliateCommissionValue { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.IsCommissionPaid")]
            public bool IsCommissionPaid { get; set; }

            [NopResourceDisplayName("Admin.Affiliates.Orders.CommissionPaidOn")]
            public DateTime? CommissionPaidOn { get; set; }

            #endregion
        }

        public partial class AffiliatedCustomerModel : BaseNopEntityModel
        {
            [NopResourceDisplayName("Admin.Affiliates.Customers.Name")]
            public string Name { get; set; }
        }

        #endregion
    }
}