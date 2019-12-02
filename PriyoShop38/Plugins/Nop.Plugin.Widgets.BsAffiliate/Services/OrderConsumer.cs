using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Services.Catalog;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.BsAffiliate.Services
{
    public class OrderConsumer : IConsumer<OrderPlacedEvent>, IConsumer<OrderPaidEvent>
    {
        private readonly IRepository<AffiliateUserCommission> _affiliateUserCommissionRepository;
        private readonly IRepository<AffiliatedOrderCommission> _affiliateOrderCommissionRepository;
        private readonly IRepository<AffiliateCommissionRate> _affiliateCommissionRateRepository;
        private readonly BsAffiliateSettings _bsAffiliateSettings;
        private readonly ICategoryService _categoryService;

        public OrderConsumer(IRepository<AffiliateUserCommission> affiliateUserCommissionRepository,
            IRepository<AffiliatedOrderCommission> affiliateOrderCommissionRepository,
            BsAffiliateSettings bsAffiliateSettings,
            IRepository<AffiliateCommissionRate> affiliateCommissionRateRepository,
            ICategoryService categoryService)
        {
            _affiliateUserCommissionRepository = affiliateUserCommissionRepository;
            _affiliateOrderCommissionRepository = affiliateOrderCommissionRepository;
            _bsAffiliateSettings = bsAffiliateSettings;
            _affiliateCommissionRateRepository = affiliateCommissionRateRepository;
            _categoryService = categoryService;
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            if (eventMessage.Order.AffiliateId != 0)
            {
                var totalCommission = decimal.Zero;
                foreach (var item in eventMessage.Order.OrderItems)
                {
                    var commissionRate = GetCommissionRate(item.Product, eventMessage.Order.AffiliateId);
                    var productCommission = GetProductCommission(commissionRate.CommissionRate, commissionRate.CommissionType, item.UnitPriceInclTax * item.Quantity);
                    totalCommission += productCommission;
                }

                var commission = new AffiliatedOrderCommission()
                {
                    AffiliateId = eventMessage.Order.AffiliateId,
                    OrderId = eventMessage.Order.Id,
                    PaymentStatus = CommissionPaymentStatus.Unpaid,
                    TotalCommission = totalCommission,
                };
                _affiliateOrderCommissionRepository.Insert(commission);
            }
        }

        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            if (eventMessage.Order.AffiliateId != 0)
            {
                var totalCommission = decimal.Zero;
                foreach (var item in eventMessage.Order.OrderItems)
                {
                    var commissionRate = GetCommissionRate(item.Product, eventMessage.Order.AffiliateId);
                    var productCommission = GetProductCommission(commissionRate.CommissionRate, commissionRate.CommissionType, item.UnitPriceInclTax * item.Quantity);
                    totalCommission += productCommission;
                }

                var commission = _affiliateOrderCommissionRepository.Table.Where(x => x.OrderId == eventMessage.Order.Id).FirstOrDefault();
                if (commission != null)
                {
                    commission.TotalCommission = totalCommission;
                    _affiliateOrderCommissionRepository.Update(commission);
                }
            }
        }

        private decimal GetProductCommission(decimal commissionRate, CommissionType type, decimal orderTotal)
        {
            if (type == CommissionType.Fixed)
                return commissionRate;
            else
                return orderTotal * commissionRate / 100;
        }

        private ConnissionModel GetCommissionRate(Product product, int affiliateId)
        {
            var model = new ConnissionModel();
            var rates = _affiliateCommissionRateRepository.Table;
            var vendorRate = rates.FirstOrDefault(x => x.EntityType == EntityType.Vendor &&
                x.EntityId == product.VendorId);

            if (vendorRate != null && vendorRate.CommissionRate >= 3)
            {
                model.CommissionRate = vendorRate.CommissionRate;
                model.CommissionType = vendorRate.CommissionType;
                return model;
            }

            foreach (var category in product.ProductCategories)
            {
                var ids = new List<int>();
                ids.Add(category.CategoryId);
                GetParentCategory(category.CategoryId, ids);

                var catRate = rates.FirstOrDefault(x => ids.Contains(x.EntityId) && x.EntityType == EntityType.Category);
                if (catRate != null)
                {
                    model.CommissionType = catRate.CommissionType;
                    model.CommissionRate = catRate.CommissionRate;
                    return model;
                }
            }

            var userCommissionRate = _affiliateUserCommissionRepository.Table
                .Where(x => x.AffiliateId == affiliateId)
                .FirstOrDefault();

            if (userCommissionRate == null)
            {
                model.CommissionRate = _bsAffiliateSettings.DefaultCommission;
                model.CommissionType = _bsAffiliateSettings.CommissionType;
            }
            else
            {
                model.CommissionRate = userCommissionRate.CommissionRate;
                model.CommissionType = userCommissionRate.CommissionType;
            }
            
            return model;
        }

        private void GetParentCategory(int categoryId, List<int> ids)
        {
            int id = _categoryService.GetCategoryById(categoryId).ParentCategoryId;
            if (id == 0)
                return;
            ids.Add(id);
            GetParentCategory(id, ids);
        }
    }
}
