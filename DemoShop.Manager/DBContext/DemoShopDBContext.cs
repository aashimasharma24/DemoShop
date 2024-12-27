using DemoShop.Core.DataObjects;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DemoShop.Manager.DBContext
{
    public class DemoShopDbContext : DbContext
    {
        public DemoShopDbContext(DbContextOptions<DemoShopDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }

}
