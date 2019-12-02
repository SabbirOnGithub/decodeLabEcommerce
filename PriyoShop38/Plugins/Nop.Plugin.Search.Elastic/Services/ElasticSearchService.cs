using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Search.Elastic.Domain;
using Nop.Services.Catalog;
using Nest;
using Elasticsearch.Net;
using Nop.Services.Logging;
using Nop.Core.Caching;
using Nop.Services.Localization;
using Nop.Core.Domain.Catalog;
using System.Net;
using System.Reflection;
using Nop.Core.Data;
using Nop.Data;

namespace Nop.Plugin.Search.Elastic.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private const string ConnectioName = "Elastic";
        private const string ElastiCconnectioName = "Search.Elastic";
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryService _categoryService;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;
        private readonly ElasticLowLevelClient _elasticLowClient;
        private readonly ElasticClient _elasticClient;
       
        private readonly int _bulkSize;        
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ElasticSearchSettings _elasticSearchSettings;
        private readonly Core.Data.IRepository<Product> _productRepository;

        public ElasticSearchService(
             ILogger logger,            
            ILocalizationService localizationService,
            ICacheManager cacheManager,
            ICategoryService categoryService,            
            IProductService productService,
            IManufacturerService manufacturerService,
            ElasticSearchSettings elasticSearchSettings,
            Core.Data.IRepository<Product> productRepository
            )
        {
            _logger = logger;
            _cacheManager = cacheManager;
            _categoryService = categoryService;
            _productService = productService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _elasticSearchSettings = elasticSearchSettings;
            _productRepository = productRepository;

            #region elstic low client

          
            var node = new Uri(_elasticSearchSettings.ElasticSearchServerUrl);
            var connectionPool = new SniffingConnectionPool(new[] { node });
            var config = new ConnectionConfiguration(connectionPool);
            _elasticLowClient = new ElasticLowLevelClient(config);

            #endregion

            #region elastic high client
          
            var settings = new ConnectionSettings(node);
            _elasticClient = new ElasticClient(settings);
            #endregion
           
           _bulkSize = _elasticSearchSettings.ElasticBulkSize;
        }

        #region utilities
        private string IndexBody()
        {

            string q = string.Empty;


            q += @"{
""settings"" : {
        
    ""analysis"": {
                    ""analyzer"": {
                        ""my_analyzer"": {
                            ""type"": ""custom"",
          ""tokenizer"": ""standard"",
          ""filter"": [
            ""lowercase"",
            ""mynGram""
          ]
    }
},
      ""search_analyzer"": {
        ""my_search_analyzer"": {
          ""type"": ""custom"",
          ""tokenizer"": ""standard"",
          ""filter"": [
            ""standard"",
            ""lowercase"",
            ""mynGram""
          ]
        }
      },
      ""filter"": {
        ""mynGram"": {
          ""type"": ""nGram"",
          ""min_gram"": 2,
          ""max_gram"": 50
        }
      
    }
  }
    },

     ""mappings"": {
            ""elasticsearchproductdocument"": {
                ""properties"": {
                    
                    ""Deleted"": {
                        ""type"": ""boolean""
                    },
                    ""Description"": {
                        ""type"": ""string""
                    },
                    ""DisplayOrder"": {
                        ""type"": ""long""
                    },
                    ""Gtin"": {
                        ""type"": ""string""
                    },
                   
                    ""Id"": {
                        ""type"": ""long""
                    },
                    ""InStock"": {
                        ""type"": ""boolean""
                    },
                    
                     ""Price"": {
                        ""type"": ""double""
                    },
                    
                    ""ManufacturerName"": {
                        ""type"": ""string"",
                                ""index"": ""analyzed"",
                                 ""analyzer"":""my_analyzer""
                    },
                    
                    ""MetaDescription"": {
                        ""type"": ""string"",
                                ""index"": ""analyzed"",
                                 ""analyzer"":""my_analyzer""
                    },
                    ""MetaKeywords"": {
                        ""type"": ""string"",
                                ""index"": ""analyzed"",
                                 ""analyzer"":""my_analyzer""
                    },
                    ""MetaTitle"": {
                        ""type"": ""string"",
                                ""index"": ""analyzed"",
                                 ""analyzer"":""my_analyzer""
                    },
                    
                    
                   
                    ""ProductName"": {
                        ""type"": ""string"",
                                ""index"": ""analyzed"",
                                 ""analyzer"":""my_analyzer"",
                        ""fields"": {
                            ""raw"": {
                                ""type"": ""string"",
                                ""index"": ""not_analyzed""
                            },
 ""ac"": {
                                ""type"": ""string"",
                                ""index"": ""analyzed"",
                                 ""analyzer"":""my_analyzer""
                                
                            }

                        }
                    },
                    ""Published"": {
                        ""type"": ""boolean""
                    },
                    ""ShortDescription"": {
                        ""type"": ""string""
                    },
                   
                    ""Sku"": {
                        ""type"": ""string""
                    },
                    
                    ""StockQuantity"": {
                        ""type"": ""long""
                    },
                   
                    ""TagNames"": {
                        ""type"": ""string""
                    },
                   ""CategoryNames"": {
                        ""type"": ""string"",
                                ""index"": ""analyzed"",
                                 ""analyzer"":""my_analyzer""
                    }
                    
                   
                }
            }
        }
    }";


            return q;
        }
        private ElasticProduct PrepareElasticProduct(Product product)
        {
            var tagNames = product.ProductTags.Aggregate(string.Empty, (current, tag) => current + (tag.Name + ","));
            var categoryNames = string.Empty;
            foreach (var pCategory in product.ProductCategories)
            {
                var category = _categoryService.GetCategoryById(pCategory.CategoryId);
                if (category != null)
                {
                    categoryNames = categoryNames + category.Name + ",";
                }

            }
            var brandNames = string.Empty;
            foreach (var pManufacturer in product.ProductManufacturers)
            {
                var brand = _manufacturerService.GetManufacturerById(pManufacturer.ManufacturerId);
                if (brand != null)
                {
                    brandNames = brandNames + brand.Name + ",";
                }

            }


            var document = new ElasticProduct
            {
                Id = product.Id,
                ProductName = product.Name,
                Description = product.FullDescription,
                DisplayOrder = product.DisplayOrder,
                ShortDescription = product.ShortDescription,
                Gtin = product.Gtin,
                Sku = product.Sku,
                InStock = product.StockQuantity > 0,
                StockQuantity = product.StockQuantity,
                MetaDescription = product.MetaDescription,
                MetaKeywords = product.MetaKeywords,
                MetaTitle = product.MetaTitle,
                Published = product.Published,
                Deleted = product.Deleted,
                 ManufacturerName = brandNames,               
                TagNames = tagNames,
                CategoryNames = categoryNames,
                Price=product.Price
            };



            return document;
        }
        private string GetIndexName(string postfix)
        {
            string indexName = string.IsNullOrEmpty(_elasticSearchSettings.StoreName)
                ? string.Format("{0}.{1}", _elasticSearchSettings.ProviderName, postfix)
                : string.Format("{0}.{1}.{2}", _elasticSearchSettings.ProviderName, _elasticSearchSettings.StoreName, postfix);

            return indexName.ToLower();
        }

        private void ExportListOfDocuments(IEnumerable<ElasticSearchDocument> documents, string indexName, IndexTypeEnum type)
        {
            var bulkBody = new List<object>();

            var elasticSearchDocuments = documents as ElasticProduct[] ?? documents.ToArray();
            if (elasticSearchDocuments.Any())
            {
                int counter = 0;

                foreach (var doc in elasticSearchDocuments)
                {
                    bulkBody.Add(
                        new
                        {
                            index =
                                new
                                {
                                    _index = indexName,
                                    _type = type.ToString().ToLower(),
                                    _id = doc.Id

                                }
                        });

                    // Convert the document to dictionary
                    Dictionary<string, object> docAsDictionary = doc.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(doc, null));

                    bulkBody.Add(docAsDictionary);
                    counter++;

                    // Each x (= bulkSize) documents we bulk it to elastic
                    if (counter == _bulkSize)
                    {
                        BulkInsert(indexName, bulkBody);
                        bulkBody = new List<object>();
                        counter = 0;
                    }
                }

                // For the rest...
                BulkInsert(indexName, bulkBody);
            }
        }

        private bool BulkInsert(string indexName, PostData<object> bulkBody)
        {
            // Execute the bulk with all docs to the index
            ElasticsearchResponse<DynamicResponse> result = _elasticLowClient.Bulk<DynamicResponse>(index: indexName, body: bulkBody);

            if (!result.Success || result.HttpStatusCode != (int)HttpStatusCode.OK)
            {
                _logger.Warning(string.Format("bulk to index {0} failed.", indexName));
                if (result.OriginalException != null)
                {
                    throw result.OriginalException;
                }
                return false;
            }

            return true;
        }

        private SortDescriptor<ElasticProduct> SortSelector(ProductSortingEnum orderBy = ProductSortingEnum.Position)
        {
            var sortDesciptor = new SortDescriptor<ElasticProduct>();


            if (orderBy == ProductSortingEnum.Position)
            {
                //otherwise sort by name

                sortDesciptor = sortDesciptor.Field("_score", Nest.SortOrder.Descending)
                    .Field("DisplayOrder", Nest.SortOrder.Descending);
            }
            else if (orderBy == ProductSortingEnum.NameAsc)
            {
                //Name: A to Z
                sortDesciptor = sortDesciptor.Field("ProductName.raw", Nest.SortOrder.Ascending);
            }
            else if (orderBy == ProductSortingEnum.NameDesc)
            {
                //Name: Z to A
                sortDesciptor = sortDesciptor.Field("ProductName.raw", Nest.SortOrder.Descending); ;
            }
            else if (orderBy == ProductSortingEnum.PriceAsc)
            {
                //Price: Low to High
                sortDesciptor = sortDesciptor.Field("Price", Nest.SortOrder.Ascending);
            }
            else if (orderBy == ProductSortingEnum.PriceDesc)
            {
                //Price: High to Low
                sortDesciptor = sortDesciptor.Field("Price", Nest.SortOrder.Descending);
            }
            else if (orderBy == ProductSortingEnum.CreatedOn)
            {
                //creation date
                sortDesciptor = sortDesciptor.Field("CreatedOnUtc", Nest.SortOrder.Descending); ;
            }
            else
            {
                //actually this code is not reachable
                sortDesciptor = sortDesciptor.Field("_score", Nest.SortOrder.Descending)
                    .Field("DisplayOrder", Nest.SortOrder.Descending); ;
            }




            return sortDesciptor;
        }
        private void PrepareResults(ElasticSearchResults result,
           IReadOnlyCollection<ElasticProduct> documents, out int total)
        {
            total = documents.Count;
            // int hitsIndex = response.d.Keys.ToList().IndexOf("hits");
            if (documents.Count < 0) return;
            result.ResultsIds.AddRange(documents.Select(x => x.Id).Distinct().ToList());

           
        }
        private string PrepareSearchKeyJsonQuery(string searchKey)
        {

            string q = string.Empty;
            if (string.IsNullOrEmpty(searchKey))
                return q;


            q += @"{""match"": {
         ""ProductName"": {

                ""query"":""" + searchKey + @"""
         }
        } }," + @" {
                     ""match"": {
            ""Sku"": {
                ""query"": """ + searchKey + @""",
                ""minimum_should_match"": ""100%""
            }

        }

    },
    {
    ""match"": {
    ""Gtin"": {
    ""query"": """ + searchKey + @""",
    ""minimum_should_match"": ""100%""
}
                    
}
                 
},";

            q += @"{ ""multi_match"": {
    ""_name"": ""named_query"",
    ""boost"": 1.1,
    ""query"":""" + searchKey + @""",
    ""analyzer"": ""standard"",
    ""fuzzy_rewrite"": ""constant_score_boolean"",
    ""fuzziness"": ""AUTO"",
    ""cutoff_frequency"": 0.001,
    ""prefix_length"": 5,
    ""max_expansions"": 50,
    ""slop"": 2,
    ""lenient"": true,
 ""lenient"": true,
    ""tie_breaker"": 1.1,
    ""minimum_should_match"": 1,
 ""type"":       ""most_fields"",
    ""operator"": ""or"",
    ""fields"": [
      ""ProductName^50"",
       ""MetaTitle^5"",
      ""MetaKeywords^4"",
       ""TagNames^3"",
        ""CategoryNames^3"",
      ""MetaDescription"",
      ""ShortDescription^0.1"",
       ""Description^0.1"",
     ""ManufacturerName^50""
      

    ],
    ""zero_terms_query"": ""all""
  }
}";


            return q;
        }
        #endregion
        public bool SyncProducts()
        {
           

            try
            {
               
               string  indexName = GetIndexName(IndexTypeEnum.ElasticProduct.ToString().ToLower());
                
                #region upload data
               
                var pazeIndex = 0;
                var hasNextPage = true;
                do
                {
                    List<ElasticProduct> documents = new List<ElasticProduct>();
                    var products = _productService.SearchProducts(showHidden:false, visibleIndividuallyOnly:true, pageIndex: pazeIndex, pageSize: 1000);

                    documents.AddRange(products.Select(PrepareElasticProduct));
                    hasNextPage = products.HasNextPage;
                    pazeIndex++;
                    ExportListOfDocuments(documents, indexName, IndexTypeEnum.ElasticProduct);
                } while (hasNextPage);
                 #endregion



                

               
                return true;
                //}
            }
            catch (Exception e)
            {
                _logger.Warning(string.Format("ElaticSearchService - Sync data"), e);
                throw;
            }
        }
        public bool ReIndex()
        {
            // Build the index name
            string indexName = GetIndexName( IndexTypeEnum.ElasticProduct.ToString().ToLower());

            var response = _elasticClient.DeleteIndex(indexName);

            // Create the index with the name, settings and mappings.
           
            ElasticsearchResponse<DynamicResponse> result = _elasticLowClient.IndicesCreate<DynamicResponse>(indexName, IndexBody());


            return  result.Success;
        }

        public ElasticSearchResults Search(string searchKey, int from = 0, int size = 24)
        {
            // Initialize
            ElasticSearchResults result = new ElasticSearchResults();
           
            string indexName = GetIndexName(IndexTypeEnum.ElasticProduct.ToString().ToLower());

            ISearchResponse<ElasticProduct> response;


            //.Field("_score", Nest.SortOrder.Descending)
            //.Field("DisplayOrder", Nest.SortOrder.Descending)
            #region query
            response = _elasticClient.Search<ElasticProduct>(
                s => s.Index(indexName)
                .Type("")
             .From(from)
             .Size(size)
             .Sort(ss => SortSelector())
            .Query(q => q
               .Bool(b => b
                   .Must(
                       m => m.Bool(b2 => b2.Should(s2 => s2.Raw(PrepareSearchKeyJsonQuery(searchKey))))                       
                      )

               )
   )
   .Aggregations(x => x.Max("Price_Max", d => d.Field(f => f.Price))
                     .Min("Price_Min", d => d.Field(f => f.Price))
             ));
            #endregion

            var total = 0;
            // If for some reason search failed.
          //  if (response.Documents.Count == 0 && !response.IsValid)
                if (response.Documents.Count==0 )
            {
                //_logger.Warning(string.Format("ArtizoneSearchService - Search [indexName]{0}:[httpStatusCode]{1}:[body]{2}:[error]{3}", indexName, response.ServerError, response.Documents, response.OriginalException));
               // if (response.OriginalException != null) { throw response.OriginalException; }
                total = 0;
                return result;
            }
            
            PrepareResults(result, response.Documents, out total);
            result.Total = response.Total;
            return result;
        }

        /// <summary>
        /// Get products by identifiers
        /// </summary>
        /// <param name="productIds">Product identifiers</param>
        /// <returns>Products</returns>
        public virtual IList<Product> GetProductsByIds(int[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
                return new List<Product>();

            var query = from p in _productRepository.Table
                        where productIds.Contains(p.Id) && !p.Deleted
                        select p;
            var products = query.ToList();
            //sort by passed identifiers
            var sortedProducts = new List<Product>();
            foreach (int id in productIds)
            {
                var product = products.Find(x => x.Id == id);
                if (product != null)
                    sortedProducts.Add(product);
            }
            return sortedProducts;
        }
    }
}
