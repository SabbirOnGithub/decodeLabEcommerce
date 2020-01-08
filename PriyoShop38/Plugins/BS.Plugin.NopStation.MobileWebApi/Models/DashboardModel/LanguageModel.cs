using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.BsWebApi.Models.DashboardModel
{
    public partial class LanguageModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string FlagImageFileName { get; set; }

        public string SeoCode { get; set; }

    }
}