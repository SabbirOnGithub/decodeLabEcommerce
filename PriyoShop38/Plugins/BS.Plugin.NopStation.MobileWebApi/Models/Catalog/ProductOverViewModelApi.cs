using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class ProductOverViewModelApi : BaseNopEntityModel
    {
        public ProductOverViewModelApi()
        {
            ProductPrice = new ProductPriceModel();
            DefaultPictureModel = new PictureModel();
            ReviewOverviewModel = new ProductReviewOverviewModel();
        }

        public string Name { get; set; }

        public bool IsFreeShipping { get; set; }

        public string ShortDescription { get; set; }
        //price
        public ProductPriceModel ProductPrice { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }
        //price
        public ProductReviewOverviewModel ReviewOverviewModel { get; set; }

		#region Nested Classes

        public partial class ProductPriceModel 
        {
            public string OldPrice { get; set; }
            public string Price {get;set;}
            public decimal DiscountPercent { get; set; }
        }
        
        public partial class ProductReviewOverviewModel 
        {
            public int ProductId { get; set; }
            public int RatingSum { get; set; }
            public bool AllowCustomerReviews { get; set; }
            public int TotalReviews { get; set; }
        }
		#endregion
    }
}
