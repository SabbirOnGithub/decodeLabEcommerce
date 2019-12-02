using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.BsWebApi.Models.DashboardModel
{
    public partial class CurrencyModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string CurrencySymbol { get; set; }
    }
}