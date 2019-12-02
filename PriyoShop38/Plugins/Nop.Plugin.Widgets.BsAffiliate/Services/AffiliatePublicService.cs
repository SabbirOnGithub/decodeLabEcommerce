using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public class AffiliatePublicService : IAffiliatePublicService
    {
        private readonly string bKashSettingStringFormat = "Affiliate.Customer.BKashNumber-{0}";

        private readonly ILogger _logger;
        private readonly IAffiliateCustomerMapService _affiliateCustomerMapService;
        private readonly IAffiliateService _affiliateService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IRepository<AffiliateCustomerMapping> _acMappingRepository;
        private readonly IRepository<AffiliatedOrderCommission> _aocRepository;
        private readonly IWorkContext _workContext;

        public AffiliatePublicService(IRepository<GenericAttribute> gaRepository,
            IRepository<AffiliateCustomerMapping> acMappingRepository,
            ICustomerService customerService, ILogger logger,
            IAffiliateCustomerMapService affiliateCustomerMapService, IWorkContext workContext,
            IAffiliateService affiliateService, IOrderService orderService,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter,
            IRepository<AffiliatedOrderCommission> aocRepository,
            ISettingService settingService)
        {
            _acMappingRepository = acMappingRepository;
            _customerService = customerService;
            _logger = logger;
            _affiliateCustomerMapService = affiliateCustomerMapService;
            _affiliateService = affiliateService;
            _workContext = workContext;
            _orderService = orderService;
            _dateTimeHelper = dateTimeHelper;
            _aocRepository = aocRepository;
            _priceFormatter = priceFormatter;
            _settingService = settingService;
        }

        public AffiliatePublicDetailsModel GetCuurentCustomerAffiliatedOrders(AffiliatedOrderListModel model)
        {
            try
            {
                var result = new AffiliatePublicDetailsModel();
                var affiliateMap = _acMappingRepository.Table.FirstOrDefault(x => x.CustomerId == _workContext.CurrentCustomer.Id);
                var affiliate = _affiliateService.GetAffiliateById(affiliateMap.AffiliateId);

                if (affiliate == null)
                    throw new ArgumentException("No affiliate found with the specified id");

                DateTime? startDateValue = (model.StartDate == null) ? null
                               : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

                DateTime? endDateValue = (model.EndDate == null) ? null
                                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

                var orderStatusIds = model.OrderStatusId > 0 ? new List<int>() { model.OrderStatusId } : null;
                var paymentStatusIds = model.PaymentStatusId > 0 ? new List<int>() { model.PaymentStatusId } : null;
                var shippingStatusIds = model.ShippingStatusId > 0 ? new List<int>() { model.ShippingStatusId } : null;

                var orders = _orderService.SearchOrders(
                    createdFromUtc: startDateValue,
                    createdToUtc: endDateValue,
                    osIds: orderStatusIds,
                    psIds: paymentStatusIds,
                    ssIds: shippingStatusIds,
                    affiliateId: affiliate.Id);

                var paid = decimal.Zero;
                var total = decimal.Zero;
                var unPaid = decimal.Zero;

                foreach (var order in orders)
                {
                    var affiliateCommission = _aocRepository.Table.FirstOrDefault(x => x.OrderId == order.Id);
                    if (affiliateCommission != null)
                    {
                        result.Orders.Add(new AffiliatedOrderModel()
                        {
                            OrderDate = order.CreatedOnUtc,
                            AffiliateCommission = affiliateCommission.TotalCommission,
                            AffiliateCommissionString = _priceFormatter.FormatPrice(affiliateCommission.TotalCommission, true, false),
                            CommissionPaymentStatus = affiliateCommission.PaymentStatus,
                            MarkedAsPaidOn = affiliateCommission.MarkedAsPaidOn,
                            OrderId = order.Id,
                            OrderTotalString = _priceFormatter.FormatPrice(order.OrderTotal, true, false),
                            OrderPaymentStatus = order.PaymentStatus,
                            OrderStatus = order.OrderStatus
                        });

                        total += affiliateCommission.TotalCommission;
                        if (order.PaymentStatus == PaymentStatus.Paid)
                        {
                            if (affiliateCommission.PaymentStatus == CommissionPaymentStatus.Paid)
                                paid += affiliateCommission.TotalCommission;
                            else
                                unPaid += affiliateCommission.TotalCommission;
                        }
                    }
                }
                result.Paid = paid == 0 ? _priceFormatter.FormatPrice(paid, true, false) + " 0" : _priceFormatter.FormatPrice(paid, true, false);
                result.Unpaid = unPaid == 0 ? _priceFormatter.FormatPrice(unPaid, true, false) + " 0" : _priceFormatter.FormatPrice(unPaid, true, false);
                result.Total = total == 0 ? _priceFormatter.FormatPrice(total, true, false) + " 0" : _priceFormatter.FormatPrice(total, true, false);
                result.Payable = (paid + unPaid) == 0 ? _priceFormatter.FormatPrice(paid + unPaid, true, false) + " 0" : _priceFormatter.FormatPrice(paid + unPaid, true, false);
                result.Active = true;

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new AffiliatePublicDetailsModel();
            }
        }

        public SaveResponseModel SaveInfo(AffiliateModel model)
        {
            try
            {
                var response = new SaveResponseModel();
                var affiliateMap = _acMappingRepository.Table.FirstOrDefault(x => x.CustomerId == _workContext.CurrentCustomer.Id);
                Affiliate affiliate = null;

                if (affiliateMap != null)
                {
                    affiliate = _affiliateService.GetAffiliateById(affiliateMap.AffiliateId);
                    if (affiliate != null)
                    {
                        affiliate.Address.Address1 = model.Address.Address1;
                        affiliate.Address.Address2 = model.Address.Address2;
                        affiliate.Address.Company = model.Address.Company;
                        affiliate.Address.Email = model.Address.Email;
                        affiliate.Address.City = model.Address.City;
                        affiliate.Address.CountryId = model.Address.CountryId;
                        affiliate.Address.FaxNumber = model.Address.FaxNumber;
                        affiliate.Address.FirstName = model.Address.FirstName;
                        affiliate.Address.LastName = model.Address.LastName;
                        affiliate.Address.PhoneNumber = model.Address.PhoneNumber;
                        affiliate.Address.StateProvinceId = model.Address.StateProvinceId;
                        affiliate.Address.ZipPostalCode = model.Address.ZipPostalCode;

                        _settingService.SetSetting(string.Format(bKashSettingStringFormat, affiliate.Id), model.BKash);

                        var url = affiliate.ValidateFriendlyUrlName(model.FriendlyUrlName);
                        affiliate.FriendlyUrlName = url;
                        _affiliateService.UpdateAffiliate(affiliate);

                        var afRole = _affiliateCustomerMapService.GetAffiliateRole();

                        _workContext.CurrentCustomer.CustomerRoles.Add(afRole);
                        _workContext.CurrentCustomer.AffiliateId = affiliate.Id;
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                        response.Status = true;
                        return response;
                    }
                    else
                        _acMappingRepository.Delete(affiliateMap);
                }
                affiliate = new Affiliate()
                {
                    Active = false,
                    Address = new Address()
                    {
                        Address1 = model.Address.Address1,
                        Address2 = model.Address.Address2,
                        Company = model.Address.Company,
                        Email = model.Address.Email,
                        City = model.Address.City,
                        CountryId = model.Address.CountryId,
                        CreatedOnUtc = DateTime.UtcNow,
                        FaxNumber = model.Address.FaxNumber,
                        FirstName = model.Address.FirstName,
                        LastName = model.Address.LastName,
                        PhoneNumber = model.Address.PhoneNumber,
                        StateProvinceId = model.Address.StateProvinceId,
                        ZipPostalCode = model.Address.ZipPostalCode
                    }
                };

                _settingService.SetSetting(string.Format(bKashSettingStringFormat, affiliate.Id), model.BKash);

                var friendlyUrlName = affiliate.ValidateFriendlyUrlName(model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;

                _affiliateService.InsertAffiliate(affiliate);

                affiliateMap = new AffiliateCustomerMapping()
                {
                    AffiliateId = affiliate.Id,
                    CustomerId = _workContext.CurrentCustomer.Id
                };
                _acMappingRepository.Insert(affiliateMap);

                var role = _affiliateCustomerMapService.GetAffiliateRole();

                _workContext.CurrentCustomer.CustomerRoles.Add(role);
                _workContext.CurrentCustomer.AffiliateId = affiliate.Id;
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                response.Status = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new SaveResponseModel() { Message = "Failed to save affiliate details.", Status = false };
            }
        }
    }
}
