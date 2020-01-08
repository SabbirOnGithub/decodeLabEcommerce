using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.SSLCommerzEmi.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            PrefferedCardTypes = new List<string>();
            AvailableCardTypes = new List<string>();
        }

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.HidePaymentMethodForAmountLessThan")]
        public decimal HidePaymentMethodForAmountLessThan { get; set; }
        public bool HidePaymentMethodForAmountLessThan_OverrideForStore { get; set; }
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.StoreId")]
        public string StoreId { get; set; }
        public bool StoreId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.StorePassword")]
        public string StorePassword { get; set; }
        public bool StorePassword_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotals_OverrideForStore { get; set; }

        public IList<string> PrefferedCardTypes { get; set; }
        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.PrefferedCardType")]
        public IList<string> AvailableCardTypes { get; set; }
        public string[] CheckedCardTypes { get; set; }
        public bool PrefferedCardTypes_OverrideForStore { get; set; }
        

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.EnableIpn")]
        public bool EnableIpn { get; set; }
        public bool EnableIpn_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeeThreeMonthOption")]
        public decimal AdditionalFeeThreeMonthOption { get; set; }
        public bool AdditionalFeeThreeMonthOption_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeeSixMonthOption")]
        public decimal AdditionalFeeSixMonthOption { get; set; }
        public bool AdditionalFeeSixMonthOption_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeeNineMonthOption")]
        public decimal AdditionalFeeNineMonthOption { get; set; }
        public bool AdditionalFeeNineMonthOption_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeeTwelveMonthOption")]
        public decimal AdditionalFeeTwelveMonthOption { get; set; }
        public bool AdditionalFeeTwelveMonthOption_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeeEighteenMonthOption")]
        public decimal AdditionalFeeEighteenMonthOption { get; set; }
        public bool AdditionalFeeEighteenMonthOption_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeeTwentyFourMonthOption")]
        public decimal AdditionalFeeTwentyFourMonthOption { get; set; }
        public bool AdditionalFeeTwentyFourMonthOption_OverrideForStore { get; set; }



        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.SSLCommerzEmi.Fields.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage")]
        public bool ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage { get; set; }
        public bool ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore { get; set; }
    }
}
