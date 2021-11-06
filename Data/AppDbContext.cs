using Microsoft.EntityFrameworkCore;
using SimplzKeyGenVerifier.Models;

namespace SimplzKeyGenVerifier.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Log>? Logs { get; set; }
        public DbSet<Licence>? Licence { get; set; }
    }
}
