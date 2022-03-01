namespace m151_backend.Entities
{
    public class Run
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public decimal Duration { get; set; }
        public decimal Length { get; set; }
        public Guid? GpxFileId { get; set; }
    }
}
