using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Payments.EblSkyPay.Models
{
    public class EblSkyPayConfigurationModel : BaseNopModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string MerchantId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerPhone { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string MerchantName { get; set; }
        public string Address1{ get; set; }
        public string Address2 { get; set; }
        public string Description { get; set; }
        public bool UseSandBox { get; set; }
        public bool AutoRedirectEnable { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public string FailUrl { get; set; }
        public string ErrorCallbackUrl { get; set; }
        public string SessionId { get; set; }
        public string DisplayControl { get; set; }
        public DataLayerTrxModel TrxModel { get; set; }
        public bool ErrorInCreateOrder { get; set; }
    }
}