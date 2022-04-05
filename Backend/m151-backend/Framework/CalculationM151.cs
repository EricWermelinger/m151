﻿using m151_backend.DTOs;
using m151_backend.Entities;

namespace m151_backend.Framework
{
    public class CalculationM151
    {
        public decimal CalculateDistance(PointDTO pointA, PointDTO pointB)
        {
            decimal R = 6371;
            var lat = Math.PI / 180 * (double)(pointB.Latitude - pointA.Latitude);
            var lng = Math.PI / 180 * (double)(pointB.Longitude - pointA.Longitude);
            var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                     Math.Cos(Math.PI / 180 * (double)pointA.Latitude) * Math.Cos(Math.PI / 180 * (double)pointB.Latitude) *
                     Math.Sin(lng / 2) * Math.Sin(lng / 2);
            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));
            return R * (decimal)h2 * 1000;
        }

        public decimal CalculateRouteDistance(List<PointDTO> points)
        {
            decimal distance = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                distance += CalculateDistance(points.ElementAt(i), points.ElementAt(i + 1));
            }

            return distance;
        }

        private decimal CalculateAltitudeUp(decimal altitudeA, decimal altitudeB)
        {
            decimal up = altitudeB - altitudeA;
            if (up > 0)
            {
                return up;
            }

            return 0;
        }

        public decimal CalculateRouteAltitudeUp(List<decimal> altitudes)
        {
            decimal altitude = 0;
            for (int i = 0; i < altitudes.Count - 1; i++)
            {
                altitude += CalculateAltitudeUp(altitudes.ElementAt(i), altitudes.ElementAt(i + 1));
            }

            return altitude;
        }

        public bool RoutesEqual(List<GpxNode> routeA, List<GpxNode> routeB, decimal routeALength, decimal routeBlength)
        {
            // check runs have less or more the same length
            if (Math.Abs(routeALength - routeBlength) > 100)
            {
                return false;
            }
            return CompareRoutes(routeA, routeB) && CompareRoutes(routeB, routeA);            
        }

        private bool CompareRoutes(List<GpxNode> routeA, List<GpxNode> routeB)
        {
            // threshold -> every second, one point. -> At max 25km/h -> at most 7meters differed.
            // by adding some margin, like take care of the other side of the street etc, threshold is set to 30m.
            var THRESHOLD = 30;
            foreach (var node in routeA)
            {
                var closeNodeExists = routeB
                    .Select(n => CalculateDistance(
                        new PointDTO
                        {
                            Latitude = node.Latitude,
                            Longitude = node.Longitude
                        },
                        new PointDTO
                        {
                            Latitude = n.Latitude,
                            Longitude = n.Longitude
                        }))
                    .Any(dist => dist < THRESHOLD);
                if (!closeNodeExists)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
