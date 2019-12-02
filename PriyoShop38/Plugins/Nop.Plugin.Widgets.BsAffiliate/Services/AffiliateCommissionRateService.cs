using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Vendors;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public class AffiliateCommissionRateService : IAffiliateCommissionRateService
    {
        private readonly ILogger _logger;
        private readonly IRepository<AffiliateCommissionRate> _acrRepository;
        private readonly ICategoryService _categoryService;
        private readonly IVendorService _vendorService;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;

        public AffiliateCommissionRateService(ILogger logger,
            IRepository<AffiliateCommissionRate> acrRepository,
            ICategoryService categoryService,
            IVendorService vendorService,
            CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter)
        {
            _categoryService = categoryService;
            _logger = logger;
            _acrRepository = acrRepository;
            _vendorService = vendorService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _currencySettings = currencySettings;
        }

        public SaveResponseModel AddCommission(AffiliateCommissionRateModel model)
        {
            var response = new SaveResponseModel();
            try
            {
                var entityId = 0;
                if (model.EntityType == EntityType.Category)
                    entityId = model.CategoryId.Value;
                else if (model.EntityType == EntityType.Vendor)
                    entityId = model.VendorId.Value;

                var commission = new AffiliateCommissionRate()
                {
                    CommissionRate = model.CommissionRate,
                    CommissionType = model.CommissionType.Value,
                    EntityType = model.EntityType.Value,
                    EntityId = entityId
                };

                _acrRepository.Insert(commission);

                response.Status = true;
                response.Message = "Commission rate saved successfully.";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            response.Message = "Failed to save commission rate";
            return response;
        }

        public SaveResponseModel EditCommission(AffiliateCommissionRateModel model)
        {
            var response = new SaveResponseModel();
            try
            {
                var commission = _acrRepository.GetById(model.Id);
                if (commission == null)
                {
                    response.Status = false;
                    response.Message = "Commission not found.";
                    return response;
                }

                var entityId = 0;
                if (model.EntityType == EntityType.Category)
                    entityId = model.CategoryId.Value;
                else if (model.EntityType == EntityType.Vendor)
                    entityId = model.VendorId.Value;

                commission.CommissionRate = model.CommissionRate;
                commission.CommissionType = model.CommissionType.Value;
                commission.EntityType = model.EntityType.Value;
                commission.EntityId = entityId;

                _acrRepository.Update(commission);

                response.Status = true;
                response.Message = "Commission rate saved successfully.";
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            response.Message = "Failed to save commission rate";
            return response;
        }

        public AffiliateCommissionRateModel GetCommissionRate(int commissionRateId)
        {
            try
            {
                var commission = _acrRepository.GetById(commissionRateId);
                var model = new AffiliateCommissionRateModel()
                {
                    CategoryList = GetCategoryList(),
                    VendorList = GetVendorList(),
                    
                };
                if (commission != null)
                {
                    model.EntityType = commission.EntityType;
                    if (commission.EntityType == EntityType.Category)
                        model.CategoryId = commission.EntityId;
                    else
                        model.VendorId = commission.EntityId;
                    model.CommissionRate = commission.CommissionRate;
                    model.Id = commission.Id;
                    model.CommissionType = commission.CommissionType;
                return model;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return null;
        }

        public SelectList GetCategoryList()
        {
            try
            {
                return new SelectList(_categoryService.GetAllCategories().Select(x => new SelectListItem() { Text = x.GetFormattedBreadCrumb(_categoryService), Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList(), "Value", "Text");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new SelectList(new List<SelectListItem>());
            }
        }

        public SelectList GetVendorList()
        {
            try
            {
                return new SelectList(_vendorService.GetAllVendors().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }), "Value", "Text");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new SelectList(new List<SelectListItem>());
            }
        }

        public PagedList<AffiliateCommissionRateModel> GetAffiliateCommissions(DataSourceRequest command)
        {
            try
            {
                var data = new List<AffiliateCommissionRateModel>();

                var commissions = _acrRepository.Table.OrderByDescending(x => x.EntityType);
                foreach (var commission in commissions)
                {
                    var entityName = "";
                    if (commission.EntityType == EntityType.Category)
                    {
                        var category = _categoryService.GetCategoryById(commission.EntityId);
                        entityName = category != null ? category.GetFormattedBreadCrumb(_categoryService) : "";
                    }
                    else
                    {
                        var vendor = _vendorService.GetVendorById(commission.EntityId);
                        entityName = vendor != null ? vendor.Name : "";
                    }

                    data.Add(new  AffiliateCommissionRateModel()
                    {
                        EntityName = entityName,
                        EntityId = commission.EntityId,
                        CommissionRate = commission.CommissionRate,
                        CommissionRateString = GetCommission(commission.CommissionRate, commission.CommissionType),
                        Id = commission.Id,
                        EntityTypeString = commission.EntityType.ToString(),
                        CommissionTypeString = commission.CommissionType.ToString()
                    });
                }
                var pagedData = new PagedList<AffiliateCommissionRateModel>(data, command.Page - 1, command.PageSize);

                return pagedData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }
        }

        public void DeleteRate(int id)
        {
            try
            {
                var rate = _acrRepository.GetById(id);
                _acrRepository.Delete(rate);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
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
    }
}
