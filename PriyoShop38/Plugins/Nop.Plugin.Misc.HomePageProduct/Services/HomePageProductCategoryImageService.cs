using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Services
{
    public partial class HomePageProductCategoryImageService : IHomePageProductCategoryImageService
    {
        #region Field
        private readonly IRepository<HomePageProductCategoryImage> _homePageProductCategoryImageRepository;
        private readonly IRepository<Product> _productRepository;

        #endregion

        #region Ctr

        public HomePageProductCategoryImageService(IRepository<HomePageProductCategoryImage> homePageProductCategoryImageRepository, IRepository<Product> productRepository)
        {
            _homePageProductCategoryImageRepository = homePageProductCategoryImageRepository;
            _productRepository = productRepository;
        }

        #endregion

        #region Methods

        public void Delete(int id)
        {
            var item = _homePageProductCategoryImageRepository.GetById(id);

            //item.Deleted = true;

            _homePageProductCategoryImageRepository.Delete(item);
        }

        public bool UpdateCategoryColor(HomePageProductCategoryImage HomePageProductCategoryImage)
        {
            _homePageProductCategoryImageRepository.Update(HomePageProductCategoryImage);

            return true;
        }

        public void Insert(HomePageProductCategoryImage item)
        {
            //default value
            item.CreatedOnUtc= DateTime.UtcNow;
            item.UpdateOnUtc = DateTime.UtcNow;

            _homePageProductCategoryImageRepository.Insert(item);
        }

        public virtual IList<HomePageProductCategoryImage> GetCategoryPicturesByCategoryId(int categoryId)
        {
            var query = from cp in _homePageProductCategoryImageRepository.Table
                        where cp.CategoryId == categoryId
                        orderby cp.DisplayOrder
                        select cp;
            var categoryPictures = query.ToList();
            return categoryPictures;
        }

        public IList<int> GetPageProductCategoryImageIdByCategoryId(int categoryId)
        {
            var query = from cp in _homePageProductCategoryImageRepository.Table
                        where cp.CategoryId == categoryId
                        select cp.ImageId;
            var categoryPictures = query.ToList();
            return categoryPictures;
        }

        public HomePageProductCategoryImage HomePageProductCategoryImage(int Id)
        {
            if (Id == 0)
                return null;

            return _homePageProductCategoryImageRepository.GetById(Id);
        }


        public List<HomePageProductCategoryImage> GetHomePageProductCategoryImagesByCategoryID(int categoryId)
        {
            if (categoryId == 0)
                return null;

            var query = from cp in _homePageProductCategoryImageRepository.Table
                        where cp.CategoryId == categoryId
                        select cp;
            var categoryPictures = query.ToList();

            return categoryPictures;
            
        }

        public IList<HomePageProductCategoryImage> GetPageProductCategoryImage()
        {

           var query = from cp in _homePageProductCategoryImageRepository.Table
                    group cp by new { cp.CategoryId }
                        into categorygroup
                        select categorygroup.FirstOrDefault();

           var categoryPictures = query.OrderBy(x=>x.CategoryId).ToList();
            return categoryPictures;
        }

        public string GetPageProductCategoryColor(int categoryId)
        {

            var query = from cp in _homePageProductCategoryImageRepository.Table
                        group cp by new { cp.CategoryId }
                            into categorygroup
                            select categorygroup.FirstOrDefault();
            string categoryColor = "";
            var categoryPictures = query.Where(x => x.CategoryId == categoryId);
            if (categoryPictures.Count() > 0)
            {
                 categoryColor = categoryPictures.FirstOrDefault().CategoryColor;
            }
            return categoryColor;
        }

        public void UpdateCategoryPictureDetails(HomePageProductCategoryImage HomePageProductCategoryImage)
        {
            _homePageProductCategoryImageRepository.Update(HomePageProductCategoryImage);
        }

        public void DeleteCategoryPicture(HomePageProductCategoryImage HomePageProductCategoryImage)
        {
            _homePageProductCategoryImageRepository.Delete(HomePageProductCategoryImage);
        }

        //public void Update(BiponeeMangoPreOrder item)
        //{
        //    var model = _biponeeMangoPreOrderRepository.GetById(item.Id);

        //    _biponeeMangoPreOrderRepository.Update(model);
        //}

        //public IPagedList<BiponeeMangoPreOrder> GetAllPreOrders(int pageNuber, int pageSize)
        //{
        //    IQueryable<BiponeeMangoPreOrder> query = from i in _biponeeMangoPreOrderRepository.Table
        //                                           where !i.Deleted
        //                                           select i;

        //    query = query.OrderByDescending(x => x.CreatedOnUtc);

        //    return new PagedList<BiponeeMangoPreOrder>(query, pageNuber, pageSize);
        //}

        //public BiponeeMangoPreOrder GetById(int id)
        //{
        //    return _biponeeMangoPreOrderRepository.GetById(id);
        //}

        //public IPagedList<BiponeeMangoPreOrder> SearchPreOrders(DateTime? startTime, DateTime? endTime, string billingEmail, string customerAddress,
        //    string productName, string customerName, string preOrderStatus, int pageNumber, int pageSize = int.MaxValue)
        //{
        //    var query = from o in _biponeeMangoPreOrderRepository.Table
        //                    where !o.Deleted select o;
            
        //    //date filter
        //    if (startTime.HasValue)
        //        query = query.Where(o => startTime.Value <= o.CreatedOnUtc);
        //    if (endTime.HasValue)
        //        query = query.Where(o => endTime.Value >= o.CreatedOnUtc);

        //    //email
        //    if (!String.IsNullOrWhiteSpace(billingEmail))
        //    {
        //        query = query.Where(x => x.Email.Contains(billingEmail));
        //    }

        //    //address
        //    if (!String.IsNullOrWhiteSpace(customerAddress))
        //    {
        //        query = query.Where(x => x.CustomerAddress.Contains(customerAddress));
        //    }

        //    //product name
        //    if (!String.IsNullOrWhiteSpace(productName))
        //    {
        //        //cur ids
        //        var curIds = _biponeeMangoPreOrderRepository.Table
        //            .Where(x => !x.Deleted)
        //            .Select(x => x.ProductVariantId)
        //            .Distinct()
        //            .ToList();

        //        //get all product ids
        //        var productIds = _productRepository.Table
        //            .Where(x=>!x.Deleted)
        //            .Where(x => curIds.Any(p => p == x.Id))
        //            .Distinct()
        //            .Where(x => x.Name.Contains(productName))
        //            .Distinct()
        //            .Select(x => x.Id)
        //            .Distinct()
        //            .ToList();
                
        //        query = query.Where(x => productIds.Any(p=>p.Equals(x.ProductId)));
        //    }

        //    //search by customer name
        //    if(!string.IsNullOrWhiteSpace(customerName))
        //    {
        //        query = query.Where(x => x.CustomerName.Contains(customerName));
        //    }

        //    //search by pre order status
        //    if (!string.IsNullOrWhiteSpace(preOrderStatus))
        //    {
        //        query = query.Where(x => x.OrderStatus.Contains(preOrderStatus));
        //    }

        //    //order by
        //    query = query.OrderByDescending(x => x.CreatedOnUtc);

        //    //return
        //    return new PagedList<BiponeeMangoPreOrder>(query, pageNumber, pageSize, int.MaxValue);
        //}

        #endregion    
    }
}