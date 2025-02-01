using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroDynasty.Core.Models.Core
{
    public class Coordinates
    {
        public double Latitude;
        public double Longitude;
        public string String { get => this.ToString(); }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public override string ToString()
        {
            return $"{Latitude:F4} / {Longitude:F4}";
        }

    }

    public static class GeoUtilities
    {
        private const double _earthRadius = 6371.0; //in km

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">Degrees value.</param>
        /// <returns>Radians value.</returns>
        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        public static double CalculateDistance(Coordinates origin, Coordinates destination)
        {
            double lonOrgRad = DegreesToRadians(origin.Longitude);
            double latOrgRad = DegreesToRadians(origin.Latitude);
            double lonDesRad = DegreesToRadians(destination.Longitude);
            double latDesRad = DegreesToRadians(destination.Latitude);

            double deltaLon = lonDesRad - lonOrgRad;
            double deltaLat = latDesRad - latOrgRad;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(latOrgRad) * Math.Cos(latDesRad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = _earthRadius * c;

            return distance;
        }
    }
}
