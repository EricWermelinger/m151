namespace m151_backend.DTOs
{
    public class UserDTO
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public DateTime? Birthdate { get; set; }
        public Guid? SexId { get; set; }
    }
}
