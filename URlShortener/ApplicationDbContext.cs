using Microsoft.EntityFrameworkCore;
using URlShortener.Entity;
using URlShortener.Services;

namespace URlShortener
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder.Property(s => s.Code).HasMaxLength(UrlShortenServices.NumberOfCharsInShortUrl);
                builder.HasIndex(x => x.Code).IsUnique();
            });
        }
    }
}
