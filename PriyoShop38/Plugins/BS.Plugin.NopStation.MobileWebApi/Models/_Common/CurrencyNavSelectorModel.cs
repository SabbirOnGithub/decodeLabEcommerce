using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._Common
{
    public partial class CurrencyNavSelectorModel : BaseNopModel
    {
        public CurrencyNavSelectorModel()
        {
            AvailableCurrencies = new List<CurrencyNavModel>();
        }

        public IList<CurrencyNavModel> AvailableCurrencies { get; set; }

        public int CurrentCurrencyId { get; set; }
    }
}
