using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public static class ErrorMessageExtension
    {
        public enum ErrorMessage
        {

            [Description("Invalid AccessToken Error")]
            AccessTokenError = 1,
            [Description("Connection Problem")]
            OtherError = 2,
            [Description("More Followers Needed")]
            FollowersError = 3,
            [Description("No Error")]
            NoError = 0,
            [Description("Offer Already Taken By User")]
            OfferAlredyTaken=4,
            [Description("No Offers Taken By This User")]
            NoOfferTaken=5,
            [Description("Parameter missing")]
            ParameterMissing=6,
            [Description("Latitude longitude Misplace")]
            LatitudeLongitude=7

        }
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
