namespace m151_backend.Entities
{
    public class UserData
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public DateTime? Birthdate { get; set; }
        public Guid? SexId { get; set; }
    }
}
