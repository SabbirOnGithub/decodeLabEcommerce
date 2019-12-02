using System;
using Nop.Core;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Services
{
    public partial interface IHomePageProductService
    {
        void Delete(int categoryId);
        void Insert(HomePageProductCategory item);

        IList<int> GetHomePageProductCategoryProductIdByCategoryId(int categoryId);
        HomePageProductCategory GetHomePageProductByProductId(int productId);
        //void Update(HomePageProductCategory item);
        //IPagedList<BiponeeMangoPreOrder> GetAllPreOrders(int pageNuber, int pageSize);
        //HomePageProductCategory GetById(int id);

        //IPagedList<BiponeeMangoPreOrder> SearchPreOrders(DateTime? startTime, DateTime? endTime,string billingEmail, string customerAddress,
        //                                                        string productName,string customerName,string preOrderStatus, int pageNumber, int pageSize);
    }
}