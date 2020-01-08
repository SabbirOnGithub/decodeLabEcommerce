using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileApp.Extensions
{
    public static class DistanceExtension
    {
        static public double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }
        public static DbGeography ConvertToDbGeoGraphyPosition(Double latitude, Double longitude)
        { 
            return DbGeography.FromText(String.Format("POINT ({0} {1})", longitude, latitude), 4326);
        }
        public static double GetDistance(DbGeography pointA, DbGeography pointB)
        {
            //return pointA.Distance(pointB).GetValueOrDefault();
            return DistanceTo(pointA.Latitude.GetValueOrDefault(), pointA.Longitude.GetValueOrDefault(), pointB.Latitude.GetValueOrDefault(), pointB.Longitude.GetValueOrDefault());
        }
    }
}
