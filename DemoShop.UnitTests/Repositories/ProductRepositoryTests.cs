using Xunit;
using Moq;
using DemoShop.Core.DataObjects;
using DemoShop.Manager.Repositories.Interfaces;

namespace DemoShop.UnitTests.Repositories
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async void GetProductByIdAsync_ReturnsProduct()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Price = 10.99m,
                    Description = "Description 1",
                    Category = "Category 1"
                });

            // Act
            var product = await mockProductRepository.Object.GetByIdAsync(1);

            // Assert
            Assert.Equal(1, product.Id);
            Assert.Equal("Product 1", product.Name);
            Assert.Equal(10.99m, product.Price);
        }

        [Fact]
        public async void GetProductByIdAsync_ReturnsNull_WhenProductDoesNotExist()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((Product)null);

            // Act
            var product = await mockProductRepository.Object.GetByIdAsync(1);

            // Assert
            Assert.Null(product);
        }

        [Fact]
        public async void AddProductAsync_ReturnsProduct()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var product = new Product
            {
                Id = 1,
                Name = "Product 1",
                Price = 10.99m,
                Description = "Description 1",
                Category = "Category 1"
            };

            mockProductRepository.Setup(repo => repo.AddAsync(product))
                .ReturnsAsync(product);

            // Act
            var addedProduct = await mockProductRepository.Object.AddAsync(product);

            // Assert
            Assert.Equal(1, addedProduct.Id);
            Assert.Equal("Product 1", addedProduct.Name);
            Assert.Equal(10.99m, addedProduct.Price);
        }

        [Fact]
        public async void UpdateProductAsync_ReturnsProduct()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var product = new Product
            {
                Id = 1,
                Name = "Product 1",
                Price = 10.99m,
                Description = "Description 1",
                Category = "Category 1"
            };

            mockProductRepository.Setup(repo => repo.UpdateAsync(product))
                .Returns(Task.FromResult(product));

            // Act
            var updatedProduct = await mockProductRepository.Object.UpdateAsync(product);

            // Assert
            Assert.Equal(1, updatedProduct.Id);
            Assert.Equal("Product 1", updatedProduct.Name);
            Assert.Equal(10.99m, updatedProduct.Price);
        }

        [Fact]
        public async void DeleteProductAsync_ReturnsProduct()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var product = new Product
            {
                Id = 1,
                Name = "Product 1",
                Price = 10.99m,
                Description = "Description 1",
                Category = "Category 1"
            };

            mockProductRepository.Setup(repo => repo.DeleteAsync(product.Id))
                .Returns(Task.FromResult(product));

            // Act
            await mockProductRepository.Object.DeleteAsync(product.Id);

            // Assert database record was deleted
            Assert.Empty(mockProductRepository.Object.GetAllAsync().Result);
            mockProductRepository.Verify(repo => repo.DeleteAsync(product.Id), Times.Once());
        }

        [Fact]
        public async void GetAllProductsAsync_ReturnsProducts()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Product 1",
                        Price = 10.99m,
                        Description = "Description 1",
                        Category = "Category 1"
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Product 2",
                        Price = 20.99m,
                        Description = "Description 2",
                        Category = "Category 2"
                    }
                });

            // Act
            var products = await mockProductRepository.Object.GetAllAsync();

            // Assert
            Assert.Equal(2, products.Count());
            Assert.Equal(1, products.ElementAt(0).Id);
            Assert.Equal(2, products.ElementAt(1).Id);
        }
    }

}