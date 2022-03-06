namespace m151_backend.DTOs
{
    public class GpxFileDTO
    {
        public Guid RunId { get; set; }
        public string Filename { get; set; }
        public List<GpxNodeDTO> Nodes { get; set; }
    }
}
