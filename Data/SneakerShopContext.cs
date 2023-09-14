using Microsoft.EntityFrameworkCore;
using SneakerShopMySQL.Models;

namespace SneakerShopMySQL.Data
{
    public class SneakerShopContext : DbContext
    {
        public SneakerShopContext(DbContextOptions<SneakerShopContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Sneaker> Sneakers { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(nameof(User));
            modelBuilder.Entity<Sneaker>().ToTable(nameof(Sneaker));
            modelBuilder.Entity<Cart>().ToTable(nameof(Cart));
            modelBuilder.Entity<Inventory>().ToTable(nameof(Models.Inventory));
            modelBuilder.Entity<Order>().ToTable(nameof(Order));
            modelBuilder.Entity<OrderDetails>().ToTable(nameof(Models.OrderDetails));
        }
    }
}
