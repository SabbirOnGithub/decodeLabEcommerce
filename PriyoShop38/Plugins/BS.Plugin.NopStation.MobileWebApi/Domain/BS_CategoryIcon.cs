using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
    public partial class BS_CategoryIcon : BaseEntity
    {
        public int CategoryId { get; set; }

        public string TextPrompt { get; set; }

        public int DisplayOrder { get; set; }

        public int PictureId { get; set; }
    }
}
