using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shop.WebApi.Model;

namespace Shop.WebApi
{
    public class DatabaseContext: DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Showcase> Showcases { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
        {
               
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Product>()
                .HasMany(x=>x.Showcases)
                .WithMany(x=>x.Products);
        }
    }
}
