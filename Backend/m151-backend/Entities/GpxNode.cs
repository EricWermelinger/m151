namespace m151_backend.Entities
{
    public class GpxNode
    {
        public Guid Id { get; set; }
        public Guid GpxFileId { get; set; }
        public int OrderInFile { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Elevation { get; set; }
        public DateTime Time { get; set; }
    }
}
