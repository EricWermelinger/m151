using m151_backend.Entities;

namespace Students.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            // used for unit-testing
        }

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

        public virtual DbSet<GpxFile> GpxFiles { get; set; }
        public virtual DbSet<GpxNode> GpxNodes { get; set; }
        public virtual DbSet<Run> Runs { get; set; }
        public virtual DbSet<RunNote> RunNotes { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
