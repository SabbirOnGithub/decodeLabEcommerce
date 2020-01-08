using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Extensions;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            StatusCode = (int)ErrorType.Ok;
            ErrorList = new List<string>();
        }

        public string SuccessMessage { get; set; }
        public int StatusCode { get; set; }
        public List<string> ErrorList { get; set; }
    }
}
