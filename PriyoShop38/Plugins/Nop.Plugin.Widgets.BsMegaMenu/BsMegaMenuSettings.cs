
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.BsMegaMenu
{
    public class BsMegaMenuSettings : ISettings
    {        
        public string SetWidgetZone { get; set; }
        public bool ShowImage { get; set; }
        public string MenuType { get; set; }
        public int noOfMenufactureItems { get; set; }
    }
}