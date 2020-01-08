using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public partial class ProductTagModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public int ProductCount { get; set; }
    }
}