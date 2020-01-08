using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public partial class AffiliateConfigureService : IAffiliateConfigureService
    {
        private readonly string bKashSettingStringFormat = "Affiliate.Customer.BKashNumber-{0}";

        private readonly IRepository<AffiliateUserCommission> _affiliateUserCommissionRepository;
        private readonly IRepository<AffiliatedOrderCommission> _affiliateOrderCommissionRepository;
        private readonly IRepository<AffiliateCustomerMapping> _affiliateCustomerMappingRepository;
        private readonly IRepository<Affiliate> _affiliateRepository;
        private readonly IRepository<AffiliateType> _atRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly ILogger _logger;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _curstomerService;
        private readonly ISettingService _settingService;
        private readonly IAffiliateService _affiliateService;
        private readonly BsAffiliateSettings _bsAffiliateSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;

        public AffiliateConfigureService(ILogger logger,
            CurrencySettings currencySettings, ICurrencyService currencyService,
            IRepository<AffiliateUserCommission> affiliateUserCommissionRepository,
            IAffiliateService affiliateService, IRepository<Affiliate> affiliateRepository,
            ISettingService settingService, IRepository<Order> orderRepository,
            BsAffiliateSettings bsAffiliateSettings, ICustomerService curstomerService,
            IRepository<AffiliatedOrderCommission> affiliateOrderCommissionRepository,
            IDateTimeHelper dateTimeHelper, IPriceFormatter priceFormatter,
            IRepository<AffiliateCustomerMapping> affiliateCustomerMappingRepository,
            IRepository<AffiliateType> atRepository)
        {
            _logger = logger;
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _affiliateUserCommissionRepository = affiliateUserCommissionRepository;
            _affiliateService = affiliateService;
            _settingService = settingService;
            _bsAffiliateSettings = bsAffiliateSettings;
            _affiliateOrderCommissionRepository = affiliateOrderCommissionRepository;
            _orderRepository = orderRepository;
            _affiliateRepository = affiliateRepository;
            _dateTimeHelper = dateTimeHelper;
            _priceFormatter = priceFormatter;
            _affiliateCustomerMappingRepository = affiliateCustomerMappingRepository;
            _curstomerService = curstomerService;
            _atRepository = atRepository;
        }

        public void UpdateSettings(AffiliateConfigureModel model)
        {
            try
            {
                _bsAffiliateSettings.DefaultCommission = model.DefaultCommission;
                _bsAffiliateSettings.CommissionType = model.CommissionType;

                _settingService.SaveSetting(_bsAffiliateSettings);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public AffiliateConfigureModel LoadSettings()
        {
            var model = new AffiliateConfigureModel();
            try
            {
                model.CommissionType = _bsAffiliateSettings.CommissionType;
                model.DefaultCommission = _bsAffiliateSettings.DefaultCommission;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return model;
        }

        private string GetCommission(decimal commissionRate, CommissionType commissionType)
        {
            var result = "";
            if (commissionType == CommissionType.Percentage)
            {
                result = commissionRate + " %";
            }
            else
            {
                var currencyId = _currencySettings.PrimaryExchangeRateCurrencyId;
                var currency = new Currency();

                if (currencyId == 0)
                    currency = _currencyService.GetCurrencyById(currencyId);
                else
                    currency = _currencyService.GetAllCurrencies().FirstOrDefault();

                result = _priceFormatter.FormatPrice(commissionRate, true, false);
            }
            return result;
        }

        public IList<AffiliateUserCommissionModel> GetAllCommissions()
        {
            var model = new List<AffiliateUserCommissionModel>();
            try
            {
                var affiliates = _affiliateService.GetAllAffiliates();

                foreach (var affiliate in affiliates)
                {
                    var affiliateCommission = _affiliateUserCommissionRepository.Table.Where(x => x.AffiliateId == affiliate.Id).FirstOrDefault();
                    var cMap = _affiliateCustomerMappingRepository.Table.Where(x => x.AffiliateId == affiliate.Id).FirstOrDefault();
                    var cusName = "";
                    var cusMail = "";
                    var cusId = 0;
                    var affiliateType = "Default";
                    if (cMap != null)
                    {
                        var customer = _curstomerService.GetCustomerById(cMap.CustomerId);
                        var at = _atRepository.GetById(cMap.AffiliateTypeId);
                        if (customer != null)
                        {
                            cusName = customer.GetFullName();
                            cusMail = customer.Email;
                            cusId = cMap.CustomerId;
                            affiliateType = at != null ? at.Name : "Default";
                        }
                    }
                    model.Add(new AffiliateUserCommissionModel()
                    {
                        CommissionRate = affiliateCommission != null ? affiliateCommission.CommissionRate : 0,
                        CommissionString = affiliateCommission != null ? GetCommission(affiliateCommission.CommissionRate, affiliateCommission.CommissionType) : "",
                        CommissionType = affiliateCommission != null ? affiliateCommission.CommissionType : CommissionType.Fixed,
                        CommissionTypeString = affiliateCommission != null ? affiliateCommission.CommissionType.ToString() : "",
                        AffiliateName = affiliate.GetFullName(),
                        AffiliateEmail = affiliate.Address.Email,
                        CustomerId = cusId,
                        CustomerEmail = cusMail,
                        CustomerName = cusName,
                        Id = affiliate.Id,
                        AffiliateType = affiliateType,
                        BKash = _settingService.GetSettingByKey<string>(string.Format(bKashSettingStringFormat, affiliate.Id))
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return model;
        }

        public void UpdateCommission(AffiliateUserCommissionModel model)
        {
            try
            {
                var commission = _affiliateUserCommissionRepository.Table.Where(x => x.AffiliateId == model.Id).FirstOrDefault();
                if (commission == null)
                {
                    commission = new AffiliateUserCommission()
                    {
                        AffiliateId = model.Id,
                        CommissionRate = model.CommissionRate,
                        CommissionType = model.CommissionType,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    };
                    _affiliateUserCommissionRepository.Insert(commission);
                }
                else
                {
                    commission.AffiliateId = model.Id;
                    commission.CommissionRate = model.CommissionRate;
                    commission.CommissionType = model.CommissionType;
                    commission.UpdatedOnUtc = DateTime.UtcNow;

                    _affiliateUserCommissionRepository.Update(commission);
                }
                _settingService.SetSetting(string.Format(bKashSettingStringFormat, model.Id), model.BKash);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public void ResetCommission(int id)
        {
            try
            {
                var commission = _affiliateUserCommissionRepository.Table.Where(x => x.AffiliateId == id).FirstOrDefault();
                if (commission != null)
                    _affiliateUserCommissionRepository.Delete(commission);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public DataSourceResult GetAllOrders(DataSourceRequest command, AffiliatedOrderModel model)
        {
            try
            {
                var commissions = _affiliateOrderCommissionRepository.Table.ToList();

                var orders = from comm in commissions
                             join ord in _orderRepository.Table on comm.OrderId equals ord.Id
                             join aff in _affiliateRepository.Table on comm.AffiliateId equals aff.Id
                             select new AffiliatedOrderModel()
                             {
                                 AffiliateEmail = aff.Address.Email,
                                 AffiliateName = aff.GetFullName(),
                                 AffiliateFirstName = aff.Address.FirstName,
                                 AffiliateLastName = aff.Address.LastName,
                                 AffiliateId = aff.Id,
                                 CommissionPaymentStatus = comm.PaymentStatus,
                                 StoreId = ord.StoreId,
                                 MarkedAsPaidOn = comm.MarkedAsPaidOn,
                                 MarkedAsPaidOnString = comm.MarkedAsPaidOn.HasValue ? _dateTimeHelper.ConvertToUserTime(comm.MarkedAsPaidOn.Value, DateTimeKind.Utc).ToString() : "Not paid yet",
                                 OrderPaymentStatus = ord.PaymentStatus,
                                 OrderTotal = ord.OrderTotal,
                                 OrderTotalString = _priceFormatter.FormatPrice(ord.OrderTotal, true, false),
                                 OrderStatus = ord.OrderStatus,
                                 OrderDate = ord.CreatedOnUtc,
                                 OrderDateString = _dateTimeHelper.ConvertToUserTime(ord.CreatedOnUtc, DateTimeKind.Utc).ToString(),
                                 OrderPaymentStatusString = ord.PaymentStatus.ToString(),
                                 CommissionPaymentStatusString = comm.PaymentStatus.ToString(),
                                 OrderStatusString = ord.OrderStatus.ToString(),
                                 OrderId = ord.Id,
                                 AffiliateCommission = comm.TotalCommission,
                                 AffiliateCommissionString = _priceFormatter.FormatPrice(comm.TotalCommission, true, false),
                                 Id = comm.Id
                             };

                if (!string.IsNullOrWhiteSpace(model.AffiliateFirstName))
                    orders = orders.Where(x => x.AffiliateFirstName.ToLower().Contains(model.AffiliateFirstName.ToLower().Trim()));
                if (!string.IsNullOrWhiteSpace(model.AffiliateLastName))
                    orders = orders.Where(x => x.AffiliateLastName.ToLower().Contains(model.AffiliateLastName.ToLower().Trim()));
                if (!string.IsNullOrWhiteSpace(model.AffiliateEmail))
                    orders = orders.Where(x => x.AffiliateEmail.ToLower() == model.AffiliateEmail.ToLower().Trim());
                if (model.StoreId != 0)
                    orders = orders.Where(x => x.StoreId == model.StoreId);
                if (model.StartDate.HasValue)
                    orders = orders.Where(x => x.OrderDate >= model.StartDate);
                if (model.EndDate.HasValue)
                    orders = orders.Where(x => x.OrderDate <= model.EndDate);
                if (model.OrderStatus != 0)
                    orders = orders.Where(x => x.OrderStatus == model.OrderStatus);
                if (model.OrderPaymentStatus != 0)
                    orders = orders.Where(x => x.OrderPaymentStatus == model.OrderPaymentStatus);
                if (model.CommissionPaymentStatus != 0)
                    orders = orders.Where(x => x.CommissionPaymentStatus == model.CommissionPaymentStatus);

                var queryResult = orders.ToList();
                var total = queryResult.Select(x => x.AffiliateCommission).Sum();
                var paid = queryResult.Where(x => x.CommissionPaymentStatus == CommissionPaymentStatus.Paid).Select(x => x.AffiliateCommission).Sum();
                var unPaid = total - paid;

                var result = new DataSourceResult()
                {
                    Data = queryResult.PagedForCommand(command).ToList(),
                    Total = queryResult.Count(),
                    ExtraData = new AggreratorModel
                    {
                        paidCommission = _priceFormatter.FormatPrice(paid, true, false),
                        totalCommission = _priceFormatter.FormatPrice(total, true, false),
                        unpaidCommission = _priceFormatter.FormatPrice(unPaid, true, false)
                    }
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new DataSourceResult()
                {
                    Errors = ex.Message
                };
            }
        }

        public void MarkAsPaid(int id)
        {
            try
            {
                var commission = _affiliateOrderCommissionRepository.GetById(id);
                commission.MarkedAsPaidOn = DateTime.UtcNow;
                commission.PaymentStatus = CommissionPaymentStatus.Paid;

                _affiliateOrderCommissionRepository.Update(commission);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
