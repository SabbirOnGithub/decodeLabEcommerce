using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel
{
    //ErrorCode=0 no error
    //ErrorCode=1 authentication error 
    //ErrorCode=2 Error message Show From api 
    //ErrorCode=3 unknown error 
    public class GeneralResponseModel<TResult> : BaseResponse
    {
        public TResult Data { get; set; }
    }
}
