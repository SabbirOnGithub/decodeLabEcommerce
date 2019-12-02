using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.ShoppingCart
{
    //ErrorCode=0 no error
    //ErrorCode=1 authentication error 
    //ErrorCode=2 Error message Show From api 
    //ErrorCode=3 unknown error 
    public class GeneralResponseModel<TResult>
    {
        public GeneralResponseModel()
        {
            StatusCode = 200;
            ErrorList=new List<string>();
        }

        public string SuccessMessage { get; set; }
        public int StatusCode { get; set; }
        public List<string> ErrorList { get; set; }
        public TResult Data { get; set; }
    }


    
}
