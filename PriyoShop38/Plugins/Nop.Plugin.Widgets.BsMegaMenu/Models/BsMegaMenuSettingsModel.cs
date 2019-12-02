using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;


namespace Nop.Plugin.Widgets.BsMegaMenu.Models
{
    public class BsMegaMenuSettingsModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.BsMegaMenu.Fields.SetWidgetZone")]
        public string SetWidgetZone { get; set; }
        [NopResourceDisplayName("Plugins.BsMegaMenu.Fields.ShowImage")]
        public bool ShowImage { get; set; }
        [NopResourceDisplayName("Plugins.BsMegaMenu.Fields.MenuType")]
        public string MenuType { get; set; }
        [NopResourceDisplayName("Plugins.BsMegaMenu.Fields.NoOfMenufactureItems")]
        public int noOfMenufactureItems { get; set; }
    }
}