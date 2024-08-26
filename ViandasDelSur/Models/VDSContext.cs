using Microsoft.EntityFrameworkCore;

namespace ViandasDelSur.Models
{
    public class VDSContext: DbContext
    {
        public VDSContext(DbContextOptions<VDSContext> options) : base(options) { }

        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Delivery> Delivery { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<SaleData> SaleData { get; set; }

    }
}
