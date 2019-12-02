using System;
using Nop.Core;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Services
{
    public partial interface IHomePageProductCategoryImageService
    {
        void Insert(HomePageProductCategoryImage item);

        IList<HomePageProductCategoryImage> GetPageProductCategoryImage();
        IList<int> GetPageProductCategoryImageIdByCategoryId(int categoryId);
        IList<HomePageProductCategoryImage> GetCategoryPicturesByCategoryId(int categoryId);
        HomePageProductCategoryImage HomePageProductCategoryImage(int Id);

        List<HomePageProductCategoryImage> GetHomePageProductCategoryImagesByCategoryID(int categoryId);
        bool UpdateCategoryColor(HomePageProductCategoryImage HomePageProductCategoryImage);
        string GetPageProductCategoryColor(int categoryId);
        void DeleteCategoryPicture(HomePageProductCategoryImage HomePageProductCategoryImage);
        void UpdateCategoryPictureDetails(HomePageProductCategoryImage HomePageProductCategoryImage);

        //void Update(HomePageProductCategory item);
        //IPagedList<BiponeeMangoPreOrder> GetAllPreOrders(int pageNuber, int pageSize);
        //HomePageProductCategory GetById(int id);

        //IPagedList<BiponeeMangoPreOrder> SearchPreOrders(DateTime? startTime, DateTime? endTime,string billingEmail, string customerAddress,
        //                                                        string productName,string customerName,string preOrderStatus, int pageNumber, int pageSize);
    }
}