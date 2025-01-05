using DemoShop.Core.DataObjects;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;

namespace DemoShop.Manager.DBContext
{
    public class DemoShopDbContext : DbContext
    {
        public DemoShopDbContext(DbContextOptions<DemoShopDbContext> options) : base(options)
        {
            Log.Information("DemoShopDbContext initialized.");
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}
