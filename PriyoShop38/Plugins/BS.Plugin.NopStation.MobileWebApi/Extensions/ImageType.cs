using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public enum ImageType : int
    {
        Default = 0,
        Slider = 1,
        Banner = 2,
        Login = 3,
        HomeTop = 4,
        HomeFirst = 5,
        HomeSecond = 6
    }

    public enum CampaignType : int
    {
        Text = 1,
        Image = 2
    }
}
