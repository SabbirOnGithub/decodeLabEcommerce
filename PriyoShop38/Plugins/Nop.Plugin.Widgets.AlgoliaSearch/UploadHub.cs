using Algolia.Search;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using Nop.Services.Logging;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Nop.Core.Infrastructure;
using Nop.Core.Data;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Nop.Plugin.Widgets.AlgoliaSearch.Factories;
using System.Data;
using Nop.Data;

namespace Nop.Plugin.Widgets.AlgoliaSearch
{
    [HubName("UploadHub")]
    public class UploadHub : Hub
    {
        public IPagedList<Product> GetPagedProductsByIds(int fromId, int toId, int pageIndex, int pageSize)
        {
            var dataProvider = EngineContext.Current.Resolve<IDataProvider>();
            var dbContext = EngineContext.Current.Resolve<IDbContext>();

            var pFromId = dataProvider.GetParameter();
            pFromId.ParameterName = "FromId";
            pFromId.Value = fromId;
            pFromId.DbType = DbType.Int32;

            var pToId = dataProvider.GetParameter();
            pToId.ParameterName = "ToId";
            pToId.Value = toId;
            pToId.DbType = DbType.Int32;

            var pPageIndex = dataProvider.GetParameter();
            pPageIndex.ParameterName = "PageIndex";
            pPageIndex.Value = pageIndex;
            pPageIndex.DbType = DbType.Int32;

            var pPageSize = dataProvider.GetParameter();
            pPageSize.ParameterName = "PageSize";
            pPageSize.Value = pageSize;
            pPageSize.DbType = DbType.Int32;

            var pTotalRecords = dataProvider.GetParameter();
            pTotalRecords.ParameterName = "TotalRecords";
            pTotalRecords.Direction = ParameterDirection.Output;
            pTotalRecords.DbType = DbType.Int32;

            //invoke stored procedure
            var products = dbContext.ExecuteStoredProcedureList<Product>(
                "ProductLoadAlgoliaPaged",
                pFromId,
                pToId,
                pPageIndex,
                pPageSize,
                pTotalRecords);

            //return products
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<Product>(products, pageIndex, pageSize, totalRecords);
        }        

        [HubMethodName("UploadProducts")]
        public void UploadProducts(int fromId, int toId, Index index, ILogger logger, IProductModelFactory productModelFactory)
        {
            var ids = Enumerable.Range(fromId, toId - fromId + 1).ToArray();
            var pageNumber = 0; 
            var currentPageProducts = 0;
            var totalProducts = 0;
            var totalPages = 0;
            var uploaded = 0;
            var failed = 0;

            logger.Information("test");

            var context = GlobalHost.ConnectionManager.GetHubContext<UploadHub>();

            var removeIds = ids.Clone() as int[];

            try
            {                
                while (true)
                {
                    SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, 0, failed, uploaded, -10, "Products fetching from database...");
                    var products = GetPagedProductsByIds(fromId, toId, pageNumber, 100);
                    if (products == null || products.Count == 0)
                        break;

                    currentPageProducts = products.Count;
                    totalProducts = products.TotalCount;
                    totalPages = products.TotalPages;

                    removeIds = removeIds.Where(x => !products.Select(y => y.Id).Contains(x)).ToArray();

                    var binding = 0;
                    var objects = new List<JObject>();

                    foreach (var product in products)
                    {
                        try
                        {
                            SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, binding + 1, failed, uploaded, 110);
                            var algoliaModel = productModelFactory.PrepareAlgoliaUploadModel(product);

                            var obj = new
                            {
                                algoliaModel.AutoCompleteImageUrl,
                                algoliaModel.Categories,
                                algoliaModel.DefaultPictureModel,
                                algoliaModel.Manufacturers,
                                algoliaModel.Name,
                                algoliaModel.MarkAsNew,
                                algoliaModel.Id,
                                algoliaModel.objectID,
                                algoliaModel.ProductPrice,
                                algoliaModel.ReviewOverviewModel,
                                algoliaModel.SeName,
                                algoliaModel.ShortDescription,
                                algoliaModel.Specifications,
                                algoliaModel.Vendor,
                                algoliaModel.OldPrice,
                                algoliaModel.Price,
                                algoliaModel.ProductType,
                                algoliaModel.FullDescription,
                                algoliaModel.CustomProperties,
                                algoliaModel.Rating,
                                algoliaModel.EnableEmi,
                                algoliaModel.Sku,
                                algoliaModel.SoldOut,
                                algoliaModel.EkshopOnly
                            };

                            var jobject = JObject.FromObject(obj);
                            objects.Add(jobject);

                            binding++;
                            SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, binding, failed, uploaded, 10);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message + ", Product Id = " + product.Id, ex);
                            failed++;

                            SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, binding, failed, uploaded, -1, ex.Message);
                            continue;
                        }
                    }

                    SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, binding, failed, uploaded, 20);
                    index.PartialUpdateObjects(objects, true);

                    uploaded += binding;
                    SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, binding, failed, uploaded, 10);
                    pageNumber++;
                }

                SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, 0, failed, uploaded, -1, "Checking unpublished products...");

                if(removeIds != null && removeIds.Count() > 0)
                    index.DeleteObjects(removeIds.Select(x=> x.ToString()));

                SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, 0, failed, uploaded, 50);

                SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, 0, failed, uploaded, 100);
            }
            catch (Exception e)
            {
                logger.Error(e.Message, e);

                SendNotification(context, pageNumber, totalPages, currentPageProducts, totalProducts, 0, failed, uploaded, -1, e.Message);
            }
        }
        
        private static void SendNotification(IHubContext context, int pageNumber, int totalPages, int currentPageProducts, int totalProducts, int binding, int failed, int uploaded, int status, string message = "")
        {
            context.Clients.All.productUploaded(new
            {
                TotalProducts = totalProducts,
                UploadedProducts = uploaded,
                CurrentPageProducts = currentPageProducts,
                Binding = binding,
                CurrentPage = pageNumber + 1,
                TotalPages = totalPages,
                Failed = failed,
                Status = status,
                Message = message
            });
        }
    }
}
