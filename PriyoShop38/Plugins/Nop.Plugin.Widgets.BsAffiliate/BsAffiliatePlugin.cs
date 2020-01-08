using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using Nop.Plugin.Widgets.BsAffiliate.Extensions;
using System.Web.Routing;
using Nop.Services.Security;
using System.Linq;

namespace Nop.Plugin.Widgets.BsAffiliate
{
    public partial class BsAffiliatePlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {

        private BsAffiliateObjectContext _context;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;

        public BsAffiliatePlugin(BsAffiliateObjectContext context, 
            ISettingService settingService,
            ICustomerService customerService,
            IPermissionService permissionService)
        {
            _context = context;
            _settingService = settingService;
            _customerService = customerService;
            _permissionService = permissionService;
        }

        public override void Install()
        {
            _permissionService.InstallPermissions(new BsAffiliatePermissionProvider());

            var role = _customerService.GetCustomerRoleBySystemName(BsCustomerRoleNames.BsAffiliate);
            if (role == null)
            {
                role = new CustomerRole()
                {
                    Active = true,
                    Name = "BS Affiliate",
                    SystemName = BsCustomerRoleNames.BsAffiliate,
                };
                _customerService.InsertCustomerRole(role);
            }

            _context.Install();
            this.AddOrUpdatePluginLocaleResource("Admin.Affiliates.Fields.BKash", "BKash Number");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateLevel", "Affiliate Level");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCommission", "Affiliate Commision");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliatedOrder", "Affiliated Order");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateConfigureModel.DefaultCommission", "Default Commission");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateConfigureModel.DefaultCommission.Hint", "Define default commission");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateConfigureModel.CommissionType", "Commission Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateConfigureModel.CommissionType.Hint", "Define commission type");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.Id", "Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.AffiliateName", "Affiliate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CustomerName", "Customer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CommissionRate", "Commission Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CommissionType", "Commission Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CreatedOnUtc", "Created On");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.UpdatedOnUtc", "Updated On");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.CommissionRate", "Commission");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderId", "Order Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.CommissionPaymentStatus", "Commission Payment Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.FirstName", "First Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.LastName", "Last Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.Email", "Email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StoreId", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StartDate", "Start Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.EndDate", "End Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderTotal", "Order Total");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.MarkedAsPaidOn", "Commission Payment Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderPaymentStatus", "Order Payment Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderStatus", "Order Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderDate", "Order Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateName", "Affiliate Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateCommission", "Affiliate Commission");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderId.Hint", "Order Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.CommissionPaymentStatus.Hint", "Commission Payment Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.FirstName.Hint", "First Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.LastName.Hint", "Last Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.Email.Hint", "Email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StoreId.Hint", "Store");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StartDate.Hint", "Start Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.EndDate.Hint", "End Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderTotal.Hint", "Order Total");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.MarkedAsPaidOn.Hint", "Commission Payment Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderPaymentStatus.Hint", "Order Payment Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderStatus.Hint", "Order Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderDate.Hint", "Order Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateName.Hint", "Affiliate Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateCommission.Hint", "Affiliate Commission");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerId", "Customer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerId.Hint", "Customer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerName", "Customer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerName.Hint", "Type customer name or email address");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateName", "Affiliate Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateName.Hint", "Affiliate name");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info", "My Affiliate Details");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.VerificationPendingMessage", "Your affiliate account is not active. Please wait for admin approval.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.urlcopied", "Url copied to clidboard");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.NoData", "Order list is empty.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.OrderList", "Order List");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummary", "Summarry");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummary.Payable", "Payable");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummary.Paid", "Paid");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummary.Unpaid", "Unpaid");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummary.Total", "Total");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.FirstName.Required", "First name is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.LastName.Required", "Last name is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Email.Required", "Email is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Country.Required", "Country is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.City.Required", "City is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Address1.Required", "Address 1 is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.PhoneNumber.Required", "Phone number is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.ZipPostalCode.Required", "Zip / postal code is required");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.FirstName", "First name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.LastName", "Last name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Email", "Email");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Country", "Country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Company", "Company");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.StateProvince", "State / province");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Address2", "Address 2");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.City", "City");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Address1", "Address 1");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.PhoneNumber", "Phone number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.ZipPostalCode", "Zip postal code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.FaxNumber", "Fax number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.ApplyButton", "Apply Now");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.UpdateButton", "Update");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.ApplyTitle", "Apply For Affiliate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.ApplyTab", "Apply For Affiliate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.InfoTab", "Affiliate Info");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.OrderTab", "Affiliated Orders");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.StartDate", "Start Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.EndDate", "End Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.OrderStatus", "Order Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.PaymentStatus", "Payment Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.ShippingStatus", "Shipping Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.StartDate.Hint", "Start Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.EndDate.Hint", "End Date");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.OrderStatus.Hint", "Order Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.PaymentStatus.Hint", "Payment Status");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.ShippingStatus.Hint", "Shipping Status");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.InfoTitle", "Affiliate Account");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.OrderTitle", "Affiliated Orders");
            this.AddOrUpdatePluginLocaleResource("Admin.Affiliates.Fields.BKash", "BKash Number");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.EntityName", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.EntityId", "Type Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.Type", "Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate", "Commission");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionType", "Commission Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.VendorId", "Vendor");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CategoryId", "Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.EntityName.Hint", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.EntityId.Hint", "Type Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.Type.Hint", "Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate.Hint", "Commission");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionType.Hint", "Commission Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.VendorId.Hint", "Vendor");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CategoryId.Hint", "Category");

            this.AddOrUpdatePluginLocaleResource("admin.affiliates.fields.bkash", "BKash");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.Type", "Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.Type.Hint", "Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionType", "Commission Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionType.Hint", "Commission Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate", "Commission Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate.Hint", "Commission Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CategoryId", "Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CategoryId.Hint", "Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.VendorId", "Vendor");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.VendorId.Hint", "Vendor");
            
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.NameUrlParameter", "Name Parameter Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.IdUrlParameter", "Id Parameter Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Active", "Is Active");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Name.Hint", "Specify affiliate type ame");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.NameUrlParameter.Hint", "Specify affiliate name url parameter");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.IdUrlParameter.Hint", "Specify affiliate id url parameter");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Active.Hint", "Is active?");

            this.AddOrUpdatePluginLocaleResource("plugins.widgets.bsaffiliate.affiliatetype", "Affiliate Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateTypeId", "Affiliate Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateTypeId.Hint", "Affiliate Type");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.NameAndIdMustNoteBeSame", "Name and Id URL parameters of an affiliate type can not be same");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.NameUrlParameter.CanNotContainSpaces", "Name URL parameter of an affiliate type can not include spaces");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.IdUrlParameter.CanNotContainSpaces", "Id URL parameter of an affiliate type can not include spaces");

            base.Install();
        }

        public override void Uninstall()
        {
            _permissionService.UninstallPermissions(new BsAffiliatePermissionProvider());

            _context.UnInstall();
            this.DeletePluginLocaleResource("plugins.widgets.bsaffiliate.affiliatetype");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateTypeId");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateTypeId.Hint");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateLevel");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCommission");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateConfigureModel.DefaultCommission");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateConfigureModel.CommissionType");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.Id");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.AffiliateName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CustomerName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CommissionRate");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CommissionType");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.CreatedOnUtc");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateUserCommissionModel.UpdatedOnUtc");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.CommissionRate");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderId");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.CommissionPaymentStatus");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.FirstName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.LastName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.Email");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StoreId");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StartDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.EndDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderTotal");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.MarkedAsPaidOn");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderPaymentStatus");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderStatus");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateCommission");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.CommissionPaymentStatus.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.FirstName.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.LastName.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.Email.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StoreId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.StartDate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.EndDate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderTotal.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.MarkedAsPaidOn.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderPaymentStatus.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderStatus.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.OrderDate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateName.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateOrderModel.AffiliateCommission.Hint");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerId");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.CustomerName.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateCustomerMapModel.AffiliateName.Hint");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.VerificationPendingMessage");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.urlcopied");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.NoData");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.OrderList");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummarry");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummarry.Payable");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummarry.Paid");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummarry.Unpaid");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Info.CommissionSummarry.Total");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.FirstName.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.LastName.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Email.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Country.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.City.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Address1.Required");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.PhoneNumber.Required");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.FirstName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.LastName");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Email");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Country");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Company");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.StateProvince");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Address2");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.City");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.Address1");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.PhoneNumber");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.ZipPostalCode");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AddressModel.FaxNumber");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.ApplyButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.UpdateButton");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.ApplyTitle");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.ApplyTab");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.InfoTab");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.OrderTab");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.InActiveMessage");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.StartDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.EndDate");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.OrderStatus");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.PaymentStatus");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.ShippingStatus");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.StartDate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.EndDate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.OrderStatus.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.PaymentStatus.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.Orders.ShippingStatus.Hint");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.InfoTitle");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.OrderTitle");
            this.DeletePluginLocaleResource("Admin.Affiliates.Fields.BKash");

            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.EntityName");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.EntityId");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.Type");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionType");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.VendorId");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CategoryId");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.EntityName.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.EntityId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.Type.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionType.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.VendorId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CategoryId.Hint");

            this.DeletePluginLocaleResource("admin.affiliates.fields.bkash");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.Type");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.Type.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionType");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionType.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CommissionRate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CategoryId");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.CategoryId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.VendorId");
            this.DeletePluginLocaleResource("Plugins.Widgets.Affiliate.CommissionRate.VendorId.Hint");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.ID");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Name");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.NameUrlParameter");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.IdUrlParameter");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Active");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.ID.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Name.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.NameUrlParameter.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.IdUrlParameter.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Active.Hint");

            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.NameAndIdMustNoteBeSame");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.NameUrlParameter.CanNotContainSpaces");
            this.DeletePluginLocaleResource("Plugins.Widgets.BsAffiliate.AffiliateType.Validation.IdUrlParameter.CanNotContainSpaces");

            base.Uninstall();
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Settings";
            controllerName = "BsAffiliateConfigure";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "Nop.Plugin.Widgets.BsAffiliate.Controllers" },
                { "area", null }
            };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Public";
            controllerName = "BsAffiliate";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Widgets.BsAffiliate.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "account_navigation_after" };
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menu = new SiteMapNode()
            {
                Visible = true,
                IconClass = "fa-user-plus",
                Title = "BS Affiliate"
            };
            if (_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateConfigure))
            {
                var settings = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/BsAffiliateConfigure/Configure",
                    Title = "Settings",
                    SystemName = "Affiliate.Settings"
                };
                menu.ChildNodes.Add(settings);
            }

            if (_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageUserCommission) ||
                _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewUserCommission))
            {
                var userCommission = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/BsAffiliateConfigure/UserCommission",
                    Title = "Affiliate User Commission",
                    SystemName = "Affiliate.UserCommission"
                };
                menu.ChildNodes.Add(userCommission);
            }

            if (_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission) ||
                _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewVendorCommission))
            {
                var vendorCommission = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/BsAffiliateConfigure/VendorCommission",
                    Title = "Vendor/Category Commission",
                    SystemName = "Affiliate.VendorCommission"
                };
                menu.ChildNodes.Add(vendorCommission);
            }

            if (_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewOrder) ||
                _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageOrder))
            {
                var order = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/BsAffiliateConfigure/AffiliatedOrder",
                    Title = "Affiliated Order",
                    SystemName = "Affiliated.Order"
                };
                menu.ChildNodes.Add(order);
            }

            if (_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewAffiliateType) ||
                _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageAffiliateType))
            {
                var type = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/BsAffiliateConfigure/AffiliateType",
                    Title = "Affiliate Type",
                    SystemName = "Affiliate.Type"
                };
                menu.ChildNodes.Add(type);
            }

            if (menu.ChildNodes != null && menu.ChildNodes.Count > 0)
            {
                var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Nop Station");
                if (pluginNode != null)
                    pluginNode.ChildNodes.Add(menu);
                else
                {
                    var nopStation = new SiteMapNode()
                    {
                        Visible = true,
                        Title = "Nop Station",
                        Url = "",
                        SystemName = "Nop Station",
                        IconClass = "fa-folder-o"
                    };
                    rootNode.ChildNodes.Add(nopStation);
                    nopStation.ChildNodes.Add(menu);
                }
            }
        }
    }
}
