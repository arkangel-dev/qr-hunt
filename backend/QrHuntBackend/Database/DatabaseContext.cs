using Microsoft.EntityFrameworkCore;

namespace QrHuntBackend.Database {
    public class DatabaseContext : DbContext {

        public DatabaseContext() { }

        private IConfiguration configuration;
        public DatabaseContext(IConfiguration _configuration) {
            configuration = _configuration;
        }
        public DatabaseContext(DbContextOptions<DatabaseContext> connstr) : base(connstr) {
            //Database.EnsureCreated();
        }

        public DbSet<Entities.Game> Games { get; set; }
        public DbSet<Entities.QrCode> QrCodes { get; set; }
        public DbSet<Entities.QrCodeToUserMatch> QrCodesToUserMatch { get; set; }
        public DbSet<Entities.User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("MainApp"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Entities.QrCode>(e => {
                e.HasIndex(x => x.Content);

                e
                .HasOne(x => x.Game)
                .WithMany(x => x.Codes)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Entities.QrCodeToUserMatch>(e => {
                e
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

                e
                .HasOne(x => x.Code)
                .WithMany()
                .HasForeignKey(x => x.CodeId);

                e
                .HasIndex(x => new { x.UserId, x.CodeId })
                .IsUnique();
            });


            base.OnModelCreating(modelBuilder);
        }

    }
}
