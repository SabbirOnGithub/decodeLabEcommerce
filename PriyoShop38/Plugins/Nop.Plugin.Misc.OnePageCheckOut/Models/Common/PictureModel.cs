using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.OnePageCheckOut.Models.Common
{
    public partial class PictureModel : BaseNopModel
    {
        public string ImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}