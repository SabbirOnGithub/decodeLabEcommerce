using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.BsAffiliate.Extensions
{
    public static class AffiliateCustomerExtension
    {
        public static bool IsAffiliate(this Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return IsInCustomerRole(customer, BsCustomerRoleNames.BsAffiliate, onlyActiveCustomerRoles);
        }

        public static bool IsInCustomerRole(this Customer customer,
            string customerRoleSystemName, bool onlyActiveCustomerRoles = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (String.IsNullOrEmpty(customerRoleSystemName))
                throw new ArgumentNullException("customerRoleSystemName");

            var result = customer.CustomerRoles
                .FirstOrDefault(cr => (!onlyActiveCustomerRoles || cr.Active) && (cr.SystemName == customerRoleSystemName)) != null;
            return result;
        }

        //public static string GenerateUrl(this Affiliate affiliate, IWebHelper webHelper, AffiliateType affiliateType)
        //{
        //    if (affiliate == null)
        //        throw new ArgumentNullException("affiliate");

        //    if (webHelper == null)
        //        throw new ArgumentNullException("webHelper");

        //    var storeUrl = webHelper.GetStoreLocation(false);

        //    var url = "";
        //    if (affiliateType != null)
        //    {
        //        url = !String.IsNullOrEmpty(affiliate.FriendlyUrlName) ?
        //            //use friendly URL
        //            webHelper.ModifyQueryString(storeUrl, affiliateType.NameUrlParameter + "=" + affiliate.FriendlyUrlName, null) :
        //            //use ID
        //            webHelper.ModifyQueryString(storeUrl, affiliateType.IdUrlParameter + "=" + affiliate.Id, null);
        //    }
        //    else
        //    {
        //        url = !String.IsNullOrEmpty(affiliate.FriendlyUrlName) ?
        //            //use friendly URL
        //            webHelper.ModifyQueryString(storeUrl, "affiliate=" + affiliate.FriendlyUrlName, null) :
        //            //use ID
        //            webHelper.ModifyQueryString(storeUrl, "affiliateid=" + affiliate.Id, null);
        //    }
        //    return url;
        //}

    }
}
