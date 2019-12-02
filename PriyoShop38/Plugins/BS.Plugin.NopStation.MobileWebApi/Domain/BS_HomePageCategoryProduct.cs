using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
    public class BS_HomePageCategoryProduct : BaseEntity
    {
        public int HomePageCategoryId { get; set; }

        public int ProductId { get; set; }

        public int DisplayOrder { get; set; }

        public virtual BS_HomePageCategory HomePageCategory { get; set; }
    }
}
