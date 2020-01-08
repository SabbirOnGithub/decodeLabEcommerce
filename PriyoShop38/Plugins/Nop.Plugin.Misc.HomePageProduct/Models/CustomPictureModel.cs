using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using Nop.Web.Models.Media;

namespace Nop.Plugin.Misc.HomePageProduct.Models
{
    public partial class CustomPictureModel : PictureModel
    {
        //target url
        public string Url { get; set; }
        public string Caption { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsMainPicture { get; set; }
    }
}