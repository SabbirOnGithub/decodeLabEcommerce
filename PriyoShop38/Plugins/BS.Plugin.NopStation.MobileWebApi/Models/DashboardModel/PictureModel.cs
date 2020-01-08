using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public partial class PictureModel : BaseNopModel
    {
        public string ImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}