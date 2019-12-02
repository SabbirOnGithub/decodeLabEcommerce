using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Product
{
    public class ProductReviewQueryModel
    {
        public string ReviewText { get; set; }
        public string Title { get; set; }
        public int Rating { get; set; }
    }
}
