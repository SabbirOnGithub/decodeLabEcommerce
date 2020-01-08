using Nest;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Search.Elastic.Domain
{



    public abstract class ElasticSearchDocument : BaseEntity
    {

    }

    [ElasticsearchType(Name = "ElasticProduct")]
    public class ElasticProduct : ElasticSearchDocument
    {
        public  string ProductName { get; set; }
        public  string MetaDescription { get; set; }
        public  string MetaKeywords { get; set; }
        public  string MetaTitle { get; set; }
        public  string Description { get; set; }
        public  string ShortDescription { get; set; }
        public  string Gtin { get; set; }
        public  string Sku { get; set; }
        public  int DisplayOrder { get; set; }
        public  string ManufacturerName { get; set; }
        public  string CategoryNames { get; set; }
        public  string TagNames { get; set; }

        public  bool? InStock { get; set; }
        public  int StockQuantity { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public decimal Price { get; set; }
    }


    public class ElasticSearchResults
    {
        public ElasticSearchResults()
        {
            ResultsIds = new List<int>();
           ResultsProducts = new List<Product>();
        }

        public List<int> ResultsIds { get; set; }

      

        
        public List<Product> ResultsProducts { get; set; }
        public string Price_Min { get; set; }
        public string Price_Max { get; set; }
        public long Total { get; set; }
    }

    public enum IndexTypeEnum
    {
        ElasticProduct
    }

}
