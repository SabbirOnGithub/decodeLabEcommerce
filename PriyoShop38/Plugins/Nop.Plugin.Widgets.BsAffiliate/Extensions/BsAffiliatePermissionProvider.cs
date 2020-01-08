using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.BsAffiliate.Extensions
{
    public class BsAffiliatePermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord BsAffiliateConfigure = new PermissionRecord { Name = "Bs Affiliate. Configure", SystemName = "BsAffiliateConfigure", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateViewUserCommission = new PermissionRecord { Name = "Bs Affiliate. View User Commission", SystemName = "BsAffiliateViewUserCommission", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateManageUserCommission = new PermissionRecord { Name = "Bs Affiliate. Manage User Commission", SystemName = "BsAffiliateManageUserCommission", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateViewOrder = new PermissionRecord { Name = "Bs Affiliate. View Order", SystemName = "BsAffiliateViewOrder", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateManageOrder = new PermissionRecord { Name = "Bs Affiliate. Manage Order", SystemName = "BsAffiliateManageOrder", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateViewVendorCommission = new PermissionRecord { Name = "Bs Affiliate. View Vendor Commission", SystemName = "BsAffiliateViewVendorCommission", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateManageVendorCommission = new PermissionRecord { Name = "Bs Affiliate. Manage Vendor Configure", SystemName = "BsAffiliateManageVendorCommission", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateCustomerMap = new PermissionRecord { Name = "Bs Affiliate. Customer Map", SystemName = "BsAffiliateCustomerMap", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateManageAffiliateType = new PermissionRecord { Name = "Bs Affiliate. Manage Affiliate Type", SystemName = "BsAffiliateManageAffiliateType", Category = "BsAffiliate" };
        public static readonly PermissionRecord BsAffiliateViewAffiliateType = new PermissionRecord { Name = "Bs Affiliate. View Affiliate Type", SystemName = "BsAffiliateViewAffiliateType", Category = "BsAffiliate" };

        public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        BsAffiliateConfigure,
                        BsAffiliateViewUserCommission,
                        BsAffiliateManageUserCommission,
                        BsAffiliateViewOrder,
                        BsAffiliateManageOrder,
                        BsAffiliateViewVendorCommission,
                        BsAffiliateManageVendorCommission,
                        BsAffiliateCustomerMap,
                        BsAffiliateManageAffiliateType,
                        BsAffiliateViewAffiliateType
                    }
                }
            };
        }

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                BsAffiliateConfigure,
                BsAffiliateViewUserCommission,
                BsAffiliateManageUserCommission,
                BsAffiliateViewOrder,
                BsAffiliateManageOrder,
                BsAffiliateViewVendorCommission,
                BsAffiliateManageVendorCommission,
                BsAffiliateCustomerMap,
                BsAffiliateManageAffiliateType,
                BsAffiliateViewAffiliateType
            };
        }
    }
}
