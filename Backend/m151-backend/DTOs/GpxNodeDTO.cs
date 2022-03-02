namespace m151_backend.DTOs
{
    public class GpxNodeDTO
    {
        public int OrderInFile { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Elevation { get; set; }
        public DateTime Time { get; set; }
    }
}
