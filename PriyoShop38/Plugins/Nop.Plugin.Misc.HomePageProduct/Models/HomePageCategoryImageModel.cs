using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.HomePageProduct.Models
{
    public partial class HomePageCategoryImageModel : BaseNopEntityModel
    {
        public string CategoryName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.CategoryId")]
        [AllowHtml]
        public int CategoryId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.Add")]
        public int Add { get; set; }

        [NopResourceDisplayName("Admin.Catalog.HomePageProduct.Fields.Delete")]
        public int Delete { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureUrl")]
        public string PictureUrl { get; set; }

        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute")]
        [AllowHtml]
        public string OverrideAltAttribute { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute")]
        [AllowHtml]
        public string OverrideTitleAttribute { get; set; }

        public string Url { get; set; }
        public int DisplayOrder { get; set; }
        public string CategoryColor { get; set; }

        public string Caption { get; set; }
       
        public bool IsMainPicture { get; set; }
    }
}