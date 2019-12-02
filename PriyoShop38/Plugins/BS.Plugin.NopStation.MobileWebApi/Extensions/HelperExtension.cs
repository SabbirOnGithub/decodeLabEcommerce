using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public static class HelperExtension
    {
        private const string FilePathLocation = "Plugins/NopStation.MobileWebApi/Content/IconPackage/";
        private const string DefaultImageName = "DefaultIcon.png";
        public static Guid GetGuid(string deviceId)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(deviceId));
                Guid result = new Guid(hash);
                return result;
            }
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

        public static string IconImagePath(this string fileName,IWebHelper webHelper)
        {
            string imagesDirectoryPath = CommonHelper.MapPath("~/" + FilePathLocation);
           var filePath = Path.Combine(imagesDirectoryPath, fileName);
            string url = webHelper.GetStoreLocation()
                         + FilePathLocation;
           if (!File.Exists(filePath))
           {
                url =  url+ DefaultImageName;
               
           }
           else{
               url=url+ fileName;
           }
            return url;
        }
        /// <summary>
        /// var start = new DateTime(2010, 1, 1);
        /// var end = new DateTime(2011, 1, 1);
        /// var query = list.Where(l => l.DateValue.IsBetween(start, end));
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime dt, DateTime start, DateTime end)
        {
            return (dt >= start && dt <= end);
        }

        public static bool IsBetween(this int x, int start, int end)
        {
            return (start <= x && x <= end);
        }
    }
}
