using System.Collections.Generic;
using Nop.Web.Models.Catalog;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Product
{
    public class ProductReviewsResponseModel : BaseResponse
    {
        public ProductReviewsResponseModel()
        {
            Items = new List<ProductReviewModel>();
            AddProductReview = new AddProductReviewModel();
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public bool IsFreeShipping { get; set; }

        public IList<ProductReviewModel> Items { get; set; }
        public AddProductReviewModel AddProductReview { get; set; }
    }
    }

