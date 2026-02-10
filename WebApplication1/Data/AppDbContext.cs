using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Şimdilik boş bırakabiliriz
        // public DbSet<Hello> Hellos { get; set; }
    }
}
