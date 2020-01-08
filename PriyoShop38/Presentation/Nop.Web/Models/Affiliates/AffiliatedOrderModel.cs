using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Affiliates
{
    public class AffiliatedOrderModel : BaseNopEntityModel
    {
        public string PaymentStatus { get; set; }

        public string OrderStatus { get; set; }

        public string CreatedOn { get; set; }

        public string OrderTotal { get; set; }

        public string OrderCommission { get; set; }

        public bool CommissionPaid { get; set; }

        public string CommissionPaidOn { get; set; }
    }
}