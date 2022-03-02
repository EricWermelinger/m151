using m151_backend.Entities;

namespace Students.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<GpxFile> GpxFiles { get; set; }
        public DbSet<GpxNode> GpxNodes { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<RunNote> RunNotes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserData> UserData { get; set; }
    }
}
