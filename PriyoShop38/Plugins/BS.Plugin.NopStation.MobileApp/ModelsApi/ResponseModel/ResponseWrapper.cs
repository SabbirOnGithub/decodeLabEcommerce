using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.ModelsApi.ResponseModel
{
    //ErrorCode=0 no error
    //ErrorCode=1 authentication error 
    //ErrorCode=2 Error message Show From api 
    //ErrorCode=3 unknown error 
    public class ResponseWrapper<TResult>
    {
        public bool Success { get; set; }
        public TResult ResponseResult { get; set; }
        public string Error { get; set; }
        public int ErrorCode { get; set; }
    }
}
