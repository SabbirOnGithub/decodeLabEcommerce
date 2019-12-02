using Nop.Web.Framework;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models
{
    public class UploadProductModel
    {
        [NopResourceDisplayName("Plugin.AlgoliaSearch.UploadModel.FromId")]
        public int FromId { get; set; }

        [NopResourceDisplayName("Plugin.AlgoliaSearch.UploadModel.ToId")]
        public int ToId { get; set; }
    }
}
