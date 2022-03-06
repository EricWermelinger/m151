using m151_backend.DTOs;

namespace m151_backend.Framework
{
    public class CalculationM151
    {
        public decimal CalculateDistance(PointDTO pointA, PointDTO pointB)
        {
            var r = 6371;
            var lat = Math.Sin((double)(pointB.Latitude - pointA.Latitude) * 0.5);
            var lon = Math.Sin((double)(pointB.Longitude - pointA.Longitude) * 0.5);
            var q = Math.Pow(lat, 2) + Math.Cos((double)pointA.Latitude) * Math.Cos((double)pointB.Latitude) * Math.Pow(lon, 2);

            return (decimal)(2 * r * Math.Asin(Math.Sqrt(q)));
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
    }
}
