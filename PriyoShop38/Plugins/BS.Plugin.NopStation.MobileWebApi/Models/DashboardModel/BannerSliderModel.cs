using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class BannerSliderModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Picture")]
        [UIHint("Picture")]
        public int Picture1Id { get; set; }
        public bool Picture1Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Text")]
        [AllowHtml]
        public string Text1 { get; set; }
        public bool Text1_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Link")]
        [AllowHtml]
        public string Link1 { get; set; }
        public bool Link1_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.IsProduct")]
        public bool IsProduct1 { get; set; }
        public bool IsProduct1_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ProdOrCat")]
        public string ProductOrCategory1 { get; set; }
        public bool ProductOrCategory1_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Picture")]
        [UIHint("Picture")]
        public int Picture2Id { get; set; }
        public bool Picture2Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Text")]
        [AllowHtml]
        public string Text2 { get; set; }
        public bool Text2_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Link")]
        [AllowHtml]
        public string Link2 { get; set; }
        public bool Link2_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.IsProduct")]
        public bool IsProduct2 { get; set; }
        public bool IsProduct2_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ProdOrCat")]
        public string ProductOrCategory2 { get; set; }
        public bool ProductOrCategory2_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Picture")]
        [UIHint("Picture")]
        public int Picture3Id { get; set; }
        public bool Picture3Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Text")]
        [AllowHtml]
        public string Text3 { get; set; }
        public bool Text3_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Link")]
        [AllowHtml]
        public string Link3 { get; set; }
        public bool Link3_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.IsProduct")]
        public bool IsProduct3 { get; set; }
        public bool IsProduct3_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ProdOrCat")]
        public string ProductOrCategory3 { get; set; }
        public bool ProductOrCategory3_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Picture")]
        [UIHint("Picture")]
        public int Picture4Id { get; set; }
        public bool Picture4Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Text")]
        [AllowHtml]
        public string Text4 { get; set; }
        public bool Text4_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Link")]
        [AllowHtml]
        public string Link4 { get; set; }
        public bool Link4_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.IsProduct")]
        public bool IsProduct4 { get; set; }
        public bool IsProduct4_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ProdOrCat")]
        public string ProductOrCategory4 { get; set; }
        public bool ProductOrCategory4_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Picture")]
        [UIHint("Picture")]
        public int Picture5Id { get; set; }
        public bool Picture5Id_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Text")]
        [AllowHtml]
        public string Text5 { get; set; }
        public bool Text5_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.Link")]
        [AllowHtml]
        public string Link5 { get; set; }
        public bool Link5_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.IsProduct")]
        public bool IsProduct5 { get; set; }
        public bool IsProduct5_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.ProdOrCat")]
        public string ProductOrCategory5 { get; set; }
        public bool ProductOrCategory5_OverrideForStore { get; set; }
    }
}