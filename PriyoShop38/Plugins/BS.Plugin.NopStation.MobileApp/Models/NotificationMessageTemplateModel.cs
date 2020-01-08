using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Models.Stores;
using BS.Plugin.NopStation.MobileApp.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileApp.Models
{
    [Validator(typeof(NotificationMessageTemplateValidator))]
    public partial class NotificationMessageTemplateModel : BaseNopEntityModel, ILocalizedModel<NotificationMessageTemplateLocalizedModel>
    {
        public NotificationMessageTemplateModel()
        {
            Locales = new List<NotificationMessageTemplateLocalizedModel>();
           AvailableStores = new List<StoreModel>();
        }


        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
        

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        //[AdditionalMetadata("data-emojiable", "true")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.IsActive")]
        [AllowHtml]
        public bool IsActive { get; set; }

        public bool HasAttachedDownload { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        
        //Store mapping
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public bool LimitedToStores { get; set; }
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.AvailableStores")]
        public List<StoreModel> AvailableStores { get; set; }
        public int[] SelectedStoreIds { get; set; }
        //comma-separated list of stores used on the list page
        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.LimitedToStores")]
        public string ListOfStores { get; set; }



        public IList<NotificationMessageTemplateLocalizedModel> Locales { get; set; }
    }

    public partial class NotificationMessageTemplateLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

      

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

       
    }
}