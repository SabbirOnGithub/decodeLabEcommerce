using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class MenufactureOverViewModelApi
    {
         public MenufactureOverViewModelApi()
        {
            DefaultPictureModel = new PictureModel();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public PictureModel DefaultPictureModel ;
    }
}
