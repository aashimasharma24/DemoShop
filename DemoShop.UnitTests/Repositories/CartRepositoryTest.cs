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
    public class CartRepositoryTests
    {
        private DemoShopDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DemoShopDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name each time
                .Options;
            return new DemoShopDbContext(options);
        }

        [Fact]
        public async Task GetCartItemsAsync_ShouldReturnCartItems_ForGivenUserId()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var userId = 1;

            context.CartItems.Add(new CartItem { Id = 1, UserId = userId, ProductId = 101, Quantity = 2 });
            context.CartItems.Add(new CartItem { Id = 2, UserId = userId, ProductId = 102, Quantity = 3 });
            context.CartItems.Add(new CartItem { Id = 3, UserId = 2, ProductId = 103, Quantity = 1 });
            context.SaveChanges();

            var repository = new CartRepository(context);

            // Act
            var result = await repository.GetCartItemsAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddOrUpdateCartItemAsync_ShouldAddNewCartItem_IfNotExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var userId = 1;
            var productId = 101;
            var quantity = 2;
            var repository = new CartRepository(context);

            // Act
            await repository.AddOrUpdateCartItemAsync(userId, productId, quantity);

            // Assert
            var cartItem = context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
            Assert.NotNull(cartItem);
            Assert.Equal(quantity, cartItem.Quantity);
        }

        [Fact]
        public async Task AddOrUpdateCartItemAsync_ShouldUpdateQuantity_IfExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var userId = 1;
            var productId = 101;
            var initialQuantity = 2;
            var additionalQuantity = 3;

            context.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = initialQuantity });
            context.SaveChanges();

            var repository = new CartRepository(context);

            // Act
            await repository.AddOrUpdateCartItemAsync(userId, productId, additionalQuantity);

            // Assert
            var cartItem = context.CartItems.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
            Assert.NotNull(cartItem);
            Assert.Equal(initialQuantity + additionalQuantity, cartItem.Quantity);
        }

        [Fact]
        public async Task RemoveCartItemAsync_ShouldRemoveCartItem_IfExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var cartItemId = 1;

            context.CartItems.Add(new CartItem { Id = cartItemId, UserId = 1, ProductId = 101, Quantity = 2 });
            context.SaveChanges();

            var repository = new CartRepository(context);

            // Act
            await repository.RemoveCartItemAsync(cartItemId);

            // Assert
            var cartItem = context.CartItems.Find(cartItemId);
            Assert.Null(cartItem);
        }

        [Fact]
        public async Task ClearCartAsync_ShouldRemoveAllCartItems_ForGivenUserId()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var userId = 1;

            context.CartItems.Add(new CartItem { Id = 1, UserId = userId, ProductId = 101, Quantity = 2 });
            context.CartItems.Add(new CartItem { Id = 2, UserId = userId, ProductId = 102, Quantity = 3 });
            context.CartItems.Add(new CartItem { Id = 3, UserId = 2, ProductId = 103, Quantity = 1 });
            context.SaveChanges();

            var repository = new CartRepository(context);

            // Act
            await repository.ClearCartAsync(userId);

            // Assert
            var cartItems = context.CartItems.Where(c => c.UserId == userId).ToList();
            Assert.Empty(cartItems);
        }
    }
}
