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
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);
        }
    }

}
