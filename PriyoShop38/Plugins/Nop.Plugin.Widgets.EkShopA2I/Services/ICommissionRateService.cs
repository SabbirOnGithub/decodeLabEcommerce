using Nop.Core;
using Nop.Plugin.Widgets.EkShopA2I.Domain;

namespace Nop.Plugin.Widgets.EkShopA2I.Services
{
    public interface ICommissionRateService
    {
        EsUdcCommissionRate GetCommissionRateById(int id);

        EsUdcCommissionRate GetCommissionRateByEntityId(int entityId, EntityType entityType);

        void InsertCommissionRate(EsUdcCommissionRate commissionRate);

        void UpdateCommissionRate(EsUdcCommissionRate commissionRate);

        void DeleteCommissionRate(EsUdcCommissionRate commissionRate);
    }
}