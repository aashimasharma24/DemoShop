using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DemoShop.UnitTests.Repositories
{
    public class UserRepositoryTest
    {
        private DemoShopDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DemoShopDbContext>()
                //.UseInMemoryDatabase(databaseName: "DemoShop")
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name each time
                .Options;

            var context = new DemoShopDbContext(options);

            context.Database.EnsureCreated();

            return context;
        }

        private async Task SeedDataAsync(DemoShopDbContext context)
        {
            // Create a few users
            var user1 = new User { Id = 1, Username = "User 1", PasswordHash = "password1" };
            var user2 = new User { Id = 2, Username = "User 2", PasswordHash = "password2" };

            context.Users.Add(user1);
            context.Users.Add(user2);

            await context.SaveChangesAsync();

            // Create a few orders with associated OrderItems and Products
            var product1 = new Product { Id = 1, Name = "Product 1", Price = 10.99m, Description = "Description 1", Category = "Category 1" };
            var product2 = new Product { Id = 2, Name = "Product 2", Price = 20.99m, Description = "Description 2", Category = "Category 2" };

            var order1 = new Order
            {
                Id = 1,
                UserId = 1,
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
                UserId = 2,
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
        public async Task GetByIdAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await SeedDataAsync(context);

            var userRepository = new UserRepository(context);

            // Act
            var user = await userRepository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("User 1", user.Username);
            Assert.Equal("User", user.Role);
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            await SeedDataAsync(context);

            var userRepository = new UserRepository(context);

            // Act
            var user = await userRepository.GetByUsernameAsync("User 1");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(1, user.Id);
            Assert.Equal("User 1", user.Username);
            Assert.Equal("User", user.Role);

            // Act
            user = await userRepository.GetByUsernameAsync("User 2");

            // Assert
            Assert.NotNull(user);
            Assert.Equal(2, user.Id);
            Assert.Equal("User 2", user.Username);
            Assert.Equal("User", user.Role);

            // Act
            user = await userRepository.GetByUsernameAsync("User 3");

            // Assert
            Assert.Null(user);
        }
    }
}
