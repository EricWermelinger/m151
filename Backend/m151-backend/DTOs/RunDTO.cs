namespace m151_backend.DTOs
{
    public class RunDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public decimal Duration { get; set; }
        public decimal Length { get; set; }
        public Guid? GpxFileId { get; set; }
    }
}
