using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.BsWebApi.Models.DashboardModel
{
    public partial class CurrencySelectorModel : BaseNopModel
    {
        public CurrencySelectorModel()
        {
            AvailableCurrencies = new List<CurrencyModel>();
        }

        public IList<CurrencyModel> AvailableCurrencies { get; set; }

        public int CurrentCurrencyId { get; set; }

        public CurrencyModel CurrentCurrency { get; set; }
    }
}