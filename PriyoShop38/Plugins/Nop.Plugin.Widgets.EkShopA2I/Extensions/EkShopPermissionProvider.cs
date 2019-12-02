using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.EkShopA2I.Extensions
{
    public class EkshopPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord EkShopConfigure = new PermissionRecord { Name = "Ek-Shop. Configure", SystemName = "EkShopConfigure", Category = "EkShop" };
        public static readonly PermissionRecord EkShopViewOrders = new PermissionRecord { Name = "Ek-Shop. View Orders", SystemName = "EkShopViewOrders", Category = "EkShop" };
        public static readonly PermissionRecord EkShopManageCommission = new PermissionRecord { Name = "Ek-Shop. Manage Commission", SystemName = "EkShopManageCommission", Category = "EkShop" };
        public static readonly PermissionRecord EkShopViewCommission = new PermissionRecord { Name = "Ek-Shop. View Commission", SystemName = "EkShopViewCommission", Category = "EkShop" };
        public static readonly PermissionRecord EkShopManageVendor = new PermissionRecord { Name = "Ek-Shop. Manage Vendor", SystemName = "EkShopManageVendor", Category = "EkShop" };

        public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        EkShopConfigure,
                        EkShopManageCommission,
                        EkShopManageVendor,
                        EkShopViewCommission,
                        EkShopViewOrders
                    }
                }
            };
        }

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                EkShopConfigure,
                EkShopManageCommission,
                EkShopManageVendor,
                EkShopViewCommission,
                EkShopViewOrders
            };
        }
    }
}
