using Nop.Plugin.Widgets.EkShopA2I.Models;

namespace Nop.Plugin.Widgets.EkShopA2I.Services
{
    public interface IEkshopEpService
    {
        bool TryCreateSession(string auth_token, string ekshop_api_base_url);

        bool TrySetCart(out string redirectUrl);

        ResponseModel PlaceEkShopOrder(PlaceOrderRootModel model);

        decimal GetCommissionRate(int productId);

        bool IsVendorRestricted(int productId);
    }
}