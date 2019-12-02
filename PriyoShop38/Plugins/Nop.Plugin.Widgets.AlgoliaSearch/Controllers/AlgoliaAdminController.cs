using Algolia.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Widgets.AlgoliaSearch.Factories;
using Nop.Plugin.Widgets.AlgoliaSearch.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Controllers
{
    [AdminAuthorize]
    public class AlgoliaAdminController : BasePluginController
    {
        #region Fields
        
        private readonly AlgoliaSettings _algoliaSettings;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AlgoliaAdminController(AlgoliaSettings algoliaSettings,
            ISettingService settingService,
            ILogger logger,
            IProductModelFactory productModelFactory,
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<Vendor> vendorRepository,
            IWorkContext workContext)
        {
            this._algoliaSettings = algoliaSettings;
            this._settingService = settingService;
            this._logger = logger;
            this._productModelFactory = productModelFactory;
            this._productRepository = productRepository;
            this._categoryRepository = categoryRepository;
            this._vendorRepository = vendorRepository;
            this._manufacturerRepository = manufacturerRepository;
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        private IPagedList<Product> GetProducts(int pageIndex = 0, int pageSize = int.MaxValue - 1)
        {
            var query = _productRepository.Table.Where(x => x.RecentlyUpdated);
            query = query.OrderByDescending(x => x.UpdatedOnUtc);
            return new PagedList<Product>(query, pageIndex, pageSize);
        }

        private IPagedList<Vendor> GetVendors(int pageIndex = 0, int pageSize = int.MaxValue - 1)
        {
            var query = _vendorRepository.Table.Where(x => x.RecentlyUpdated);
            query = query.OrderByDescending(x => x.UpdatedOnUtc);
            return new PagedList<Vendor>(query, pageIndex, pageSize);
        }

        private IPagedList<Manufacturer> GetManufacturers(int pageIndex = 0, int pageSize = int.MaxValue - 1)
        {
            var query = _manufacturerRepository.Table.Where(x => x.RecentlyUpdated);
            query = query.OrderByDescending(x => x.UpdatedOnUtc);
            return new PagedList<Manufacturer>(query, pageIndex, pageSize);
        }

        private IPagedList<Category> GetCategories(int pageIndex = 0, int pageSize = int.MaxValue - 1)
        {
            var query = _categoryRepository.Table.Where(x => x.RecentlyUpdated);
            query = query.OrderByDescending(x => x.UpdatedOnUtc);
            return new PagedList<Category>(query, pageIndex, pageSize);
        }

        private void LoadSelectableProperties(SearchableAttributeListModel model)
        {
            var allProperties = new string[] {
                "Name",
                "SeName",
                "ShortDescription",
                "Sku",
                "Categories.Name",
                "Categories.SeName",
                "Manufacturers.Name",
                "Manufacturers.SeName",
                "Vendor.Name",
                "Vendor.SeName",
                "Specifications.SpecificationAttributeOption",
                "Specifications.SpecificationAttributeName"
            };
            model.SelectableProperties = allProperties;
        }

        public void SetSettings()
        {
            var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
            var index = client.InitIndex("Products");

            dynamic settings = new JObject();

            var replicas = new List<AlgoliaReplica>()
                {
                    new AlgoliaReplica()
                    {
                        Name = "name_asc",
                        SortType = "asc(Name)",
                    },
                    new AlgoliaReplica()
                    {
                        Name = "name_desc",
                        SortType = "desc(Name)",
                    },
                    new AlgoliaReplica()
                    {
                        Name = "price_asc",
                        SortType = "asc(Price)",
                    },
                    new AlgoliaReplica()
                    {
                        Name = "price_desc",
                        SortType = "desc(Price)",
                    }
                };

            settings.searchableAttributes = new JArray(new string[] {
                    "Name,SeName,ShortDescription,Sku",
                    "Categories.Name,Categories.SeName",
                    "Manufacturers.Name,Manufacturers.SeName",
                    "Vendor.Name,Vendor.SeName",
                    "Specifications.SpecificationAttributeOption,Specifications.SpecificationAttributeName",
                    "ordered(Name)"
                });

            settings.attributesForFaceting = new JArray(new string[] {
                    "Specifications.IdOption",
                    "Specifications.IdOptionGroup",
                    "Price",
                    "Categories.IdName",
                    "Categories.SeName",
                    "Categories.NameSeName",
                    "Manufacturers.IdName",
                    "Vendor.IdName",
                    "SoldOut",
                    "EnableEmi",
                    "searchable(Sku)",
                    "searchable(SeName)",
                    "Rating"
                });

            settings.replicas = new JArray(replicas.Select(x => x.Name).ToArray());
            settings.customRanking = new JArray(new string[] {
                        "asc(SoldOut)"
                    });

            index.SetSettings(settings, true);

            foreach (var replica in replicas)
            {
                var replicaIndex = client.InitIndex(replica.Name);
                dynamic replicaSettings = new JObject();

                replicaSettings.customRanking = new JArray(new string[] {
                        "asc(SoldOut)",
                        replica.SortType
                    });

                replicaSettings.ranking = new JArray(new string[] {
                        "custom",
                        "typo",
                        "geo",
                        "words",
                        "filters",
                        "proximity",
                        "attribute",
                        "exact"
                    });
                replicaSettings.searchableAttributes = settings.searchableAttributes;
                replicaSettings.attributesForFaceting = settings.attributesForFaceting;
                replicaIndex.SetSettings(replicaSettings);
            }
        }

        #endregion

        #region Methods

        #region Configuration

        [ChildActionOnly]
        public virtual ActionResult Configure()
        {
            var model = new ConfigureModel()
            {
                AdminKey = _algoliaSettings.AdminKey,
                ApplicationId = _algoliaSettings.ApplicationId,
                MonitoringKey = _algoliaSettings.MonitoringKey,
                SeachOnlyKey = _algoliaSettings.SeachOnlyKey,
                SearchBoxThumbnailSize = _algoliaSettings.SearchBoxThumbnailSize,
                PageSize = _algoliaSettings.PageSize,
                MinimumQueryLength = _algoliaSettings.MinimumQueryLength,
                HideSoldOutProducts = _algoliaSettings.HideSoldOutProducts,
                UploadSoldOutProducts = _algoliaSettings.UploadSoldOutProducts,
                UploadUnPublishedProducts = _algoliaSettings.UploadUnPublishedProducts,
                AllowEmiFilter = _algoliaSettings.AllowEmiFilter,
                AllowPriceRangeFilter = _algoliaSettings.AllowPriceRangeFilter,
                AllowRatingFilter = _algoliaSettings.AllowRatingFilter,
                AllowVendorFilter = _algoliaSettings.AllowVendorFilter,
                MaximumCategoriesShowInFilter = _algoliaSettings.MaximumCategoriesShowInFilter,
                MaximumManufacturersShowInFilter = _algoliaSettings.MaximumManufacturersShowInFilter,
                MaximumVendorsShowInFilter = _algoliaSettings.MaximumVendorsShowInFilter,
                AllowCategoryFilter = _algoliaSettings.AllowCategoryFilter,
                AllowManufacturerFilter = _algoliaSettings.AllowManufacturerFilter,
                AllowSpecificationFilter = _algoliaSettings.AllowSpecificationFilter,
                MaximumSpecificationsShowInFilter = _algoliaSettings.MaximumSpecificationsShowInFilter,
                SelectablePageSizes = _algoliaSettings.SelectablePageSizes,
                ResetSettings = false,
                AllowCustomersToSelectPageSize = _algoliaSettings.AllowCustomersToSelectPageSize,
                AllowProductSorting = _algoliaSettings.AllowProductSorting,
                AllowProductViewModeChanging = _algoliaSettings.AllowProductViewModeChanging
            };
            return View("~/Plugins/Widgets.AlgoliaSearch/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        public virtual ActionResult Configure(ConfigureModel model)
        {
            if (ModelState.IsValid)
            {
                _algoliaSettings.AdminKey = model.AdminKey;
                _algoliaSettings.ApplicationId = model.ApplicationId;
                _algoliaSettings.MonitoringKey = model.MonitoringKey;
                _algoliaSettings.SeachOnlyKey = model.SeachOnlyKey;
                _algoliaSettings.PageSize = model.PageSize;
                _algoliaSettings.SearchBoxThumbnailSize = model.SearchBoxThumbnailSize;
                _algoliaSettings.MinimumQueryLength = model.MinimumQueryLength;
                _algoliaSettings.HideSoldOutProducts = model.HideSoldOutProducts;
                _algoliaSettings.UploadSoldOutProducts = model.UploadSoldOutProducts;
                _algoliaSettings.UploadUnPublishedProducts = model.UploadUnPublishedProducts;
                _algoliaSettings.AllowEmiFilter = model.AllowEmiFilter;
                _algoliaSettings.AllowPriceRangeFilter = model.AllowPriceRangeFilter;
                _algoliaSettings.AllowRatingFilter = model.AllowRatingFilter;
                _algoliaSettings.MaximumCategoriesShowInFilter = model.MaximumCategoriesShowInFilter;
                _algoliaSettings.MaximumManufacturersShowInFilter = model.MaximumManufacturersShowInFilter;
                _algoliaSettings.MaximumVendorsShowInFilter = model.MaximumVendorsShowInFilter;
                _algoliaSettings.AllowVendorFilter = model.AllowVendorFilter;
                _algoliaSettings.AllowCategoryFilter = model.AllowCategoryFilter;
                _algoliaSettings.AllowManufacturerFilter = model.AllowManufacturerFilter;
                _algoliaSettings.AllowSpecificationFilter = model.AllowSpecificationFilter;
                _algoliaSettings.SelectablePageSizes = model.SelectablePageSizes;
                _algoliaSettings.MaximumSpecificationsShowInFilter = model.MaximumSpecificationsShowInFilter;
                _algoliaSettings.AllowCustomersToSelectPageSize = model.AllowCustomersToSelectPageSize;
                _algoliaSettings.AllowProductSorting = model.AllowProductSorting;
                _algoliaSettings.AllowProductViewModeChanging = model.AllowProductViewModeChanging;

                _settingService.SaveSetting(_algoliaSettings);

                if (model.ResetSettings)
                    SetSettings();

                if (model.ClearIndex)
                {
                    var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
                    var index = client.InitIndex("Products");
                    index.ClearIndex();

                    model.ClearIndex = false;
                }

                model.ResetSettings = false;
            }
            return View("~/Plugins/Widgets.AlgoliaSearch/Views/Configure.cshtml", model);
        }

        #endregion

        #region Upload products

        public virtual ActionResult UploadProducts()
        {
            var model = new UploadProductModel();
            return View("~/Plugins/Widgets.AlgoliaSearch/Views/UploadProducts.cshtml", model);
        }
        
        [HttpPost]
        [AdminAntiForgery]
        public virtual ActionResult UploadProducts(UploadProductModel model)
        {
            var message = "";
            var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
            var index = client.InitIndex("Products");

            var hub = new UploadHub();

            hub.UploadProducts(model.FromId, model.ToId, index, _logger, _productModelFactory);

            return Json(new { Message = message });
        }

        #endregion

        #region Synonym

        public virtual ActionResult Synonym()
        {
            var model = new AlgoliaSynonymModel();
            return View("~/Plugins/Widgets.AlgoliaSearch/Views/Synonym.cshtml", model);
        }

        [HttpPost]
        public virtual ActionResult SynonymList(DataSourceRequest command, AlgoliaSynonymModel model)
        {
            try
            {
                var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
                var index = client.InitIndex("Products");

                if (string.IsNullOrWhiteSpace(model.Synonyms))
                    model.Synonyms = "";

                var results = index.SearchSynonyms(
                     model.Synonyms,
                         new Index.SynonymType[] {
                    Index.SynonymType.SYNONYM, Index.SynonymType.SYNONYM_ONEWAY
                     },
                     command.Page - 1,
                     command.PageSize
                 );

                var data = new List<AlgoliaSynonymModel>();
                foreach (var item in results["hits"])
                {
                    if (item != null)
                    {
                        var jsonModel = JsonConvert.DeserializeObject<AlgoliaSynonymJsonModel>(item.ToString());
                        data.Add(new AlgoliaSynonymModel
                        {
                            ObjectId = jsonModel.ObjectId,
                            Synonyms = string.Join(",", jsonModel.Synonyms)
                        });
                    }
                }

                var gridModel = new DataSourceResult
                {
                    Data = data,
                    Total = int.TryParse(results["nbHits"].ToString(), out int val) ? val : 0
                };

                return Json(gridModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return Json(new DataSourceResult { Errors = ex.Message });
            }
        }
        
        [HttpPost]
        public virtual ActionResult UpdateSynonym(AlgoliaSynonymModel model)
        {
            try
            {
                var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
                var index = client.InitIndex("Products");
                var jModel = new AlgoliaSynonymJsonModel()
                {
                    ObjectId = string.IsNullOrWhiteSpace(model.ObjectId) ? Guid.NewGuid().ToString() : model.ObjectId,
                    Type = "synonym",
                };
                jModel.Synonyms = model.Synonyms.Trim().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (jModel.Synonyms.Count <2)
                    return Json(new DataSourceResult { Errors = "Please add atleast 2 comma separated synonyms." });

                var json = JsonConvert.SerializeObject(jModel);

                index.SaveSynonym(
                    jModel.ObjectId.ToString(),
                    JObject.Parse(json),
                    true
                );
                return new NullJsonResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return Json(new DataSourceResult { Errors = ex.Message });
            }
        }

        [HttpPost]
        public virtual ActionResult DeleteSynonym(AlgoliaSynonymModel model)
        {
            try
            {
                var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
                var index = client.InitIndex("Products");

                index.DeleteSynonym(model.ObjectId.ToString(), true);
                return new NullJsonResult();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return Json(new DataSourceResult { Errors = ex.Message });
            }
        }

        #endregion

        #region Searchable attribute

        public virtual ActionResult SearchableAttribute()
        {
            var model = new SearchableAttributeListModel();
            var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
            var index = client.InitIndex("Products");

            var searchableAttr = index.GetSettings();

            var searchableAttrList = JsonConvert.DeserializeObject<List<string>>(searchableAttr["searchableAttributes"].ToString());

            foreach (var attr in searchableAttrList)
            {
                if (attr.Contains(","))
                {
                    model.Attributes.Add(new SearchableAttributeListModel.SearchableAttributeModel()
                    {
                        IsOrdered = false,
                        PropertyName = attr.Replace("unordered(", "").Replace(")", ""),
                        ChangableModifier = false
                    });
                }
                else
                {
                    model.Attributes.Add(new SearchableAttributeListModel.SearchableAttributeModel()
                    {
                        IsOrdered = attr.StartsWith("ordered("),
                        PropertyName = attr.Replace("unordered(", "").Replace("ordered(", "").Replace(")", ""),
                        ChangableModifier = true
                    });
                }
            }

            LoadSelectableProperties(model);

            return View("~/Plugins/Widgets.AlgoliaSearch/Views/SearchableAttribute.cshtml", model);
        }

        [HttpPost, AdminAntiForgery]
        public virtual ActionResult SearchableAttribute(SearchableAttributeListModel model)
        {
            var props = new List<string>();
            if (model.PropertyName != null && model.PropertyName.Any())
            {
                for (int i = 0; i < model.PropertyName.Length; i++)
                {
                    if (model.PropertyName[i].Contains(","))
                    {
                        props.Add(model.PropertyName[i]);
                    }
                    else
                    {
                        if (!model.IsOrdered[i])
                            props.Add("unordered(" + model.PropertyName[i] + ")");
                        else
                            props.Add("ordered(" + model.PropertyName[i] + ")");
                    }
                }
            }

            var client = new AlgoliaClient(_algoliaSettings.ApplicationId, _algoliaSettings.AdminKey);
            var index = client.InitIndex("Products");
            dynamic settings = new JObject();
            settings.searchableAttributes = new JArray(props);
            var result  = index.SetSettings(settings, true);

            SuccessNotification("Searchable attribute has been updated successfully. It may take 1-2 minutes to update in admin panel.");

            return RedirectToAction("SearchableAttribute"); ;
        }

        #endregion

        #region Updatable items

        public ActionResult UpdatableItems()
        {
            return View("~/Plugins/Widgets.AlgoliaSearch/Views/UpdatableItems.cshtml");
        }

        [HttpPost]
        public ActionResult UpdatableProducts(DataSourceRequest command)
        {
            var products = GetProducts(command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = products.Select(x=> new UpdatableItemModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    LastUpdatedBy = x.LastUpdatedBy,
                    UpdatedOn = x.UpdatedOnUtc.AddHours(-6).ToString("dd/MM/yyyy hh:mm:ss tt")
                }),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UpdatableVendors(DataSourceRequest command)
        {
            var vendors = GetVendors(command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = vendors.Select(x => new UpdatableItemModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    LastUpdatedBy = x.LastUpdatedBy,
                    UpdatedOn = x.UpdatedOnUtc.AddHours(-6).ToString("dd/MM/yyyy hh:mm:ss tt")
                }),
                Total = vendors.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UpdatableManufacturers(DataSourceRequest command)
        {
            var manufacturers = GetManufacturers(command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = manufacturers.Select(x => new UpdatableItemModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    LastUpdatedBy = x.LastUpdatedBy,
                    UpdatedOn = x.UpdatedOnUtc.AddHours(-6).ToString("dd/MM/yyyy hh:mm:ss tt")
                }),
                Total = manufacturers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UpdatableCategories(DataSourceRequest command)
        {
            var categories = GetCategories(command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x => new UpdatableItemModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    LastUpdatedBy = x.LastUpdatedBy,
                    UpdatedOn = x.UpdatedOnUtc.AddHours(-6).ToString("dd/MM/yyyy hh:mm:ss tt")
                }),
                Total = categories.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("UpdatableItems")]
        [FormValueRequired("update-all")]
        public ActionResult UpdateAll()
        {
            _productModelFactory.UpdateAlgoliaModel();
            SuccessNotification("Algolia models updated successfully");
            return RedirectToAction("UpdatableItems");
        }

        #endregion

        #endregion
    }
}
