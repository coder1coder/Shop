using Microsoft.EntityFrameworkCore;
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
    }
}
