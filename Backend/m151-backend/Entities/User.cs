namespace m151_backend.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public DateTime? Birthdate { get; set; }
        public Guid? SexId { get; set; }
        public DateTime? RefreshExpires { get; set; }
        public string? RefreshToken { get; set; }
    }
}
