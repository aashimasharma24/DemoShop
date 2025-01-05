using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories;
using DemoShop.Manager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DemoShop.UnitTests.Repositories
{
    public class OrderRepositoryTests
    {
        private DemoShopDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DemoShopDbContext>()
                //.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name each time
                .Options;

            var context = new DemoShopDbContext(options);

            // (Optional) Ensure the database is created.
            context.Database.EnsureCreated();

            return context;
        }

        private async Task SeedDataAsync(DemoShopDbContext context)
        {
            // Create a few orders with associated OrderItems and Products
            var product1 = new Product { Id = 1, Name = "Product 1", Price = 10.99m, Description = "Description 1", Category = "Category 1" };
            var product2 = new Product { Id = 2, Name = "Product 2", Price = 20.99m, Description = "Description 2", Category = "Category 2" };

            var order1 = new Order
            {
                Id = 1,
                UserId = 100,
                Status = "Pending",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2, Product = product1 },
                    new OrderItem { ProductId = 2, Quantity = 1, Product = product2 }
                },
                ShippingAddress = "123 Main St",
                PaymentMethod = "Credit Card"

            };
            var order2 = new Order
            {
                Id = 2,
                UserId = 200,
                Status = "Shipped",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 1, Product = product1 }
                },
                ShippingAddress = "123 Main St",
                PaymentMethod = "Credit Card"
            };

            await context.Products.AddRangeAsync(product1, product2);
            await context.Orders.AddRangeAsync(order1, order2);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectOrder()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await SeedDataAsync(context);

            IOrderRepository repo = new OrderRepository(context);

            // Act
            var order = await repo.GetByIdAsync(1);

            // Assert
            Assert.NotNull(order);
            Assert.Equal(1, order.Id);
            Assert.Equal(100, order.UserId);
            Assert.Equal("Pending", order.Status);
            Assert.Equal(2, order.OrderItems.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await SeedDataAsync(context);

            IOrderRepository repo = new OrderRepository(context);

            // Act
            var order = await repo.GetByIdAsync(999); // Non-existent ID

            // Assert
            Assert.Null(order);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await SeedDataAsync(context);

            IOrderRepository repo = new OrderRepository(context);

            // Act
            var orders = await repo.GetAllAsync();

            // Assert
            Assert.NotNull(orders);
            var orderList = orders.ToList();
            Assert.Equal(2, orderList.Count);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnAllOrdersForUser()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await SeedDataAsync(context);

            IOrderRepository repo = new OrderRepository(context);

            // Act
            var ordersForUser100 = await repo.GetByUserIdAsync(100);
            var ordersForUser200 = await repo.GetByUserIdAsync(200);
            var ordersForUser300 = await repo.GetByUserIdAsync(300); // no orders

            // Assert
            Assert.Single(ordersForUser100);  // user 100 has 1 order
            Assert.Single(ordersForUser200);  // user 200 has 1 order
            Assert.Empty(ordersForUser300);   // user 300 has 0 orders
        }

        [Fact]
        public async Task AddAsync_ShouldAddOrder()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            IOrderRepository repo = new OrderRepository(context);

            var newOrder = new Order
            {
                UserId = 123,
                Status = "Pending",
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 99, Quantity = 2, Product = new Product {
                        Id = 99,
                        Name = "Temp Product",
                        Price = 5.50m ,
                        Description = "Temp Description",
                        Category = "Temp Category",
                    } }
                },
                ShippingAddress = "123 Main St",
                PaymentMethod = "Credit Card"
            };

            // Act
            var addedOrder = await repo.AddAsync(newOrder);

            // Assert
            Assert.NotNull(addedOrder);
            Assert.NotEqual(0, addedOrder.Id);
            Assert.Equal("Pending", addedOrder.Status);

            var orderInDb = await context.Orders.FindAsync(addedOrder.Id);
            Assert.NotNull(orderInDb);
            Assert.Equal(123, orderInDb.UserId);
            Assert.Single(orderInDb.OrderItems);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateStatus()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await SeedDataAsync(context);

            IOrderRepository repo = new OrderRepository(context);

            // Act
            await repo.UpdateStatusAsync(1, "Completed");
            var updatedOrder = await context.Orders.FindAsync(1);

            // Assert
            Assert.NotNull(updatedOrder);
            Assert.Equal("Completed", updatedOrder.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldNotThrow_WhenOrderNotFound()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await SeedDataAsync(context);

            IOrderRepository repo = new OrderRepository(context);

            // Act
            // We expect no exception when the order does not exist.
            await repo.UpdateStatusAsync(999, "Completed");

            // Assert
            // Ensure that nothing was updated for the existing orders
            var order1 = await context.Orders.FindAsync(1);
            var order2 = await context.Orders.FindAsync(2);

            Assert.Equal("Pending", order1.Status);
            Assert.Equal("Shipped", order2.Status);
        }
    }
}
