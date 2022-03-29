using m151_backend.Entities;

namespace Students.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GpxNode>().Property(o => o.Latitude).HasPrecision(18, 10);
            modelBuilder.Entity<GpxNode>().Property(o => o.Longitude).HasPrecision(18, 10);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<GpxFile> GpxFiles { get; set; }
        public DbSet<GpxNode> GpxNodes { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<RunNote> RunNotes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
