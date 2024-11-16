

using Microsoft.EntityFrameworkCore;

namespace QrHuntBackend.Database {
    public class DatabaseContext : DbContext {

        public DbSet<Entities.Game> Games { get; set; }
        public DbSet<Entities.QrCode> QrCodes { get; set; }
        public DbSet<Entities.QrCodeToUserMatch> QrCodesToUserMatch { get;}
        public DbSet<Entities.User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=database.db;Version=3;");
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


            base.OnModelCreating(modelBuilder);
        }

    }
}
