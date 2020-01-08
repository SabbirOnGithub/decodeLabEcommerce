using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class CategoryBaseModelApi
    {
        public int Id { get; set; }
        public String Name { get; set; }

        public int ProductCount { get; set; }
    }
}
