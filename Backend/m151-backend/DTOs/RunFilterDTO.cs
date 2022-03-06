namespace m151_backend.DTOs
{
    public class RunFilterDTO
    {
        public decimal LengthMin { get; set; }
        public decimal LengthMax { get; set; }
        public decimal AltitudeMin { get; set; }
        public decimal AltitudeMax { get; set; }
        public decimal? PointLatitude { get; set;  }
        public decimal? PointLongitude { get; set; }
        public decimal? RadiusFromPoint { get; set; }
    }
}