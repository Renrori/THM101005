using DeliveryBro.ViewModels;
using GoogleMaps.LocationServices;
using Microsoft.CodeAnalysis;
using NetTopologySuite.Geometries;

namespace DeliveryBro.Extensions
{
	public static class Map
	{
		public static double[] Location(string address)
		{
			var location = new GoogleLocationService("AIzaSyDSVv7tpJnSFG82cLsD3wazbDCksKfic_o");
			var Point= location.GetLatLongFromAddress(address);
			return new double[] { Point.Latitude, Point.Longitude };
		}
		
        public static double GetDistance(LocationViewModel? customer, LocationViewModel? store)
        {
            double earthRadiusKm = 6371; // 地球半徑，單位：公里

            double dLat = ToRadians(store.Latitude - customer.Latitude);
            double dLon = ToRadians(store.Longitude - customer.Longitude);

            double lat1 = ToRadians(customer.Latitude);
            double lat2 = ToRadians(store.Latitude);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = earthRadiusKm * c; // 兩地之間的距離，單位：公里
            return distance;
        }

        private static double ToRadians(double? degree)
        {
            return (degree ?? 0 ) * Math.PI / 180;
        }


	}
}
