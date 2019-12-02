using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class AllResponseModel: BaseResponse
    {
        public IList<CategoryNavigationModelApi> Data { get; set; }
        public int Count { get; set; }
        public bool DisplayTaxInOrderSummary { get; set; }
        public bool ShowDiscountBox { get; set; }
        public LanguageNavSelectorModel Language { get; set; }

        public CurrencyNavSelectorModel Currency { get; set; }
    }
}
