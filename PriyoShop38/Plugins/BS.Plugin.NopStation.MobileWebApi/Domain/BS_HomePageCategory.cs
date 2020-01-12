using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
    public class BS_HomePageCategory : BaseEntity
    {
        private ICollection<BS_HomePageCategoryProduct> _homePageCategoryProducts;

        public int PictureId { get; set; }

        public int CategoryId { get; set; }

        public int DisplayOrder { get; set; }

        public string TextPrompt { get; set; }

        public bool Published { get; set; }
        public bool ForBlApp { get; set; }

        public byte ApplicableFor { get; set; }

        public int? PercentValue { get; set; }

        public DateTime? ActiveStartDate { get; set; }

        public DateTime? ActiveEndDate { get; set; }

        public virtual ICollection<BS_HomePageCategoryProduct> HomePageCategoryProducts
        { 

            get { return _homePageCategoryProducts ?? (_homePageCategoryProducts = new List<BS_HomePageCategoryProduct>()); }
            protected set { _homePageCategoryProducts = value; }
        }
    }
}
