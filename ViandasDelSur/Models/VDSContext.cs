using Microsoft.EntityFrameworkCore;

namespace ViandasDelSur.Models
{
    public class VDSContext: DbContext
    {
        public VDSContext(DbContextOptions<VDSContext> options) : base(options) { }

        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
