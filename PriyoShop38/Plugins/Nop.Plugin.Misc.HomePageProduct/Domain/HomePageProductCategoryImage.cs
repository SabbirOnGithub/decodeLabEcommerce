using System;
using Nop.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Plugin.Misc.HomePageProduct.Domain
{
    public class HomePageProductCategoryImage : BaseEntity
    {
        public int ImageId { get; set; }
        public int CategoryId { get; set; }

        public string CategoryColor { get; set; }
        public string Url { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdateOnUtc { get; set; }
        //public virtual HomePageCategory HomePageCategory { get; set; }
        public string Caption { get; set; }
        public bool IsMainPicture { get; set; }

    }
}