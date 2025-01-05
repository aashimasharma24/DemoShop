using Xunit;
using Moq;
using DemoShop.Core.DataObjects;
using DemoShop.Manager.Repositories.Interfaces;

namespace DemoShop.UnitTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;

        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts()
        {
            _mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Product>() { new Product { Id = 1, Name = "Test Product" } });

            var result = await _mockRepo.Object.GetAllAsync();
            Assert.Single(result);
        }

        // TODO: Additional tests
    }
}