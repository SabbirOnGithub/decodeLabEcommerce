using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Widgets.EkShopA2I.Domain;
using Nop.Services.Catalog;
using Nop.Services.Vendors;

namespace Nop.Plugin.Widgets.EkShopA2I.Services
{
    public class CommissionRateService : ICommissionRateService
    {
        #region Fields
        
        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly IRepository<EsUdcCommissionRate> _udcCommissionRateRepository;

        #endregion

        #region Ctor

        public CommissionRateService(IVendorService vendorService,
            ICategoryService categoryService,
            IRepository<EsUdcCommissionRate> udcCommissionRateRepository)
        {
            _vendorService = vendorService;
            _categoryService = categoryService;
            _udcCommissionRateRepository = udcCommissionRateRepository;
        }

        #endregion


        public void DeleteCommissionRate(EsUdcCommissionRate commissionRate)
        {
            _udcCommissionRateRepository.Delete(commissionRate);
        }

        public EsUdcCommissionRate GetCommissionRateById(int id)
        {
            return _udcCommissionRateRepository.GetById(id);
        }

        public EsUdcCommissionRate GetCommissionRateByEntityId(int entityId, EntityType entityType)
        {
            return _udcCommissionRateRepository.Table.FirstOrDefault(x => x.Type == entityType && x.EntityId == entityId);
        }

        public void InsertCommissionRate(EsUdcCommissionRate commissionRate)
        {
            _udcCommissionRateRepository.Insert(commissionRate);
        }

        public void UpdateCommissionRate(EsUdcCommissionRate commissionRate)
        {
            _udcCommissionRateRepository.Update(commissionRate);
        }
    }
}
