using Nop.Web.Framework;

namespace Nop.Admin.Models.Media
{
    public class PictureDbToFileModel
    {
        [NopResourceDisplayName("Admin.Media.Picture.Fields.FromId")]
        public int FromId { get; set; }

        [NopResourceDisplayName("Admin.Media.Picture.Fields.ToId")]
        public int ToId { get; set; }
    }
}