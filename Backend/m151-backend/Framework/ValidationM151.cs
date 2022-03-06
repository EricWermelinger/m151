namespace m151_backend.Framework
{
    public class ValidationM151
    {
        public bool ValidateGeoNode(decimal? latitude, decimal? longitude, decimal? altitude)
        {
            return !(latitude == null || longitude == null  || altitude == null || Math.Abs(latitude ?? 0) > 90 || Math.Abs(longitude ?? 0) > 180 || (altitude ?? 0) < 0);
        }
    }
}
