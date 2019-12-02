using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._Common
{
    public partial class LanguageNavSelectorModel : BaseNopModel
    {
        public LanguageNavSelectorModel()
        {
            AvailableLanguages = new List<LanguageNavModel>();
        }

        public IList<LanguageNavModel> AvailableLanguages { get; set; }

        public int CurrentLanguageId { get; set; }

        public bool UseImages { get; set; }
    }
}
