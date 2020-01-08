using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Purchase.Offer.Domain;
using Nop.Plugin.Purchase.Offer.ViewModel;
using Nop.Services.Catalog;

namespace Nop.Plugin.Widgets.CustomFooter.Data
{
    public interface IPurchaseOfferService
    {
        bool CreateOrUpdateOffer(PurchaseOfferViewModel data);
        PurchaseOfferViewModel GetOfferDetails();
        List<PurchaseOfferOptionViewModel> GetOptions();
        AddOptionModel GetPopUpModel(int? optionId);
        void AddOrUpdateOption(AddOptionModel model);
        void DeleteOption(int id);
        PurchaseOfferOptionViewModel GetPublicInfo();
    }
}