using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Services
{
    public partial class HomePageProductService : IHomePageProductService
    {
        #region Field
        private readonly IRepository<HomePageProductCategory> _homePageProductCategoryRepository;
        private readonly IRepository<Product> _productRepository;

        #endregion

        #region Ctr

        public HomePageProductService(IRepository<HomePageProductCategory> homePageProductCategoryRepository, IRepository<Product> productRepository)
        {
            _homePageProductCategoryRepository = homePageProductCategoryRepository;
            _productRepository = productRepository;
        }

        #endregion

        #region Methods

        public void Delete(int productId)
        {
            //item.Deleted = true;

            var query = from c in  _homePageProductCategoryRepository.Table
                        where c.ProductId == productId
            select c;
            var homepageProducts = query.ToList();
            foreach (var homepageProduct in homepageProducts)
            {
                _homePageProductCategoryRepository.Delete(homepageProduct);
            }
        }

        public void Insert(HomePageProductCategory item)
        {
            //default value
            item.CreatedOnUtc= DateTime.UtcNow;
            item.UpdateOnUtc = DateTime.UtcNow;

            _homePageProductCategoryRepository.Insert(item);
        }

        public IList<int> GetHomePageProductCategoryProductIdByCategoryId(int categoryId)
        {
            var query = from p in _homePageProductCategoryRepository.Table
                        where p.CategoryId == categoryId
                        orderby p.Priority 
                        select p.ProductId;

            var categoryPictures = query.ToList();
            return categoryPictures;
        }

        public HomePageProductCategory GetHomePageProductByProductId(int productId)
        {

            HomePageProductCategory objOfHomePageProductCategory = new HomePageProductCategory();

            var query = from p in _homePageProductCategoryRepository.Table
                        where p.ProductId == productId
                        orderby p.Priority
                        select p;
            var homePageProduct = query.ToList();
            if (homePageProduct.Count > 0)
            { 
            objOfHomePageProductCategory.Id = homePageProduct.FirstOrDefault().Id;
            objOfHomePageProductCategory.CategoryId = homePageProduct.FirstOrDefault().CategoryId;
            objOfHomePageProductCategory.CreatedOnUtc = homePageProduct.FirstOrDefault().CreatedOnUtc;
            objOfHomePageProductCategory.Priority = homePageProduct.FirstOrDefault().Priority;
            objOfHomePageProductCategory.ProductId = homePageProduct.FirstOrDefault().ProductId;
            objOfHomePageProductCategory.UpdateOnUtc = homePageProduct.FirstOrDefault().UpdateOnUtc;
            }
            return objOfHomePageProductCategory;

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