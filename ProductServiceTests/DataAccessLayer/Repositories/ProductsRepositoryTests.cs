using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ProductServiceTests.DataAccessLayer.Repositories
{
    public class ProductsRepositoryTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddProduct_ShouldAddProduct()
        {
            // Arrange
            using var context = GetDbContext();
            var repository = new ProductsRepository(context);

            var product = new Product
            {
                ProductID = Guid.NewGuid(),
                ProductName = "Test Product",
                Category = "Test Category"
            };

            // Act
            var result = await repository.AddProduct(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductID, result.ProductID);
            Assert.Equal("Test Product", result.ProductName);

            var savedProduct = await context.Products.FindAsync(product.ProductID);
            Assert.NotNull(savedProduct);
        }

        [Fact]
        public async Task GetProductByCondition_ShouldReturnMatchingProduct()
        {
            // Arrange
            using var context = GetDbContext();

            var product = new Product
            {
                ProductID = Guid.NewGuid(),
                ProductName = "Test Product",
                Category = "Test Category"
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductsRepository(context);

            // Act
            var result = await repository.GetProductByCondition(
                p => p.ProductID == product.ProductID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductID, result.ProductID);
            Assert.Equal("Test Product", result.ProductName);
        }

        [Fact]
        public async Task GetProductByCondition_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            using var context = GetDbContext();
            var repository = new ProductsRepository(context);

            // Act
            var result = await repository.GetProductByCondition(
                p => p.ProductID == Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnAllProducts()
        {
            // Arrange
            using var context = GetDbContext();

            context.Products.AddRange(
                new Product
                {
                    ProductID = Guid.NewGuid(),
                    ProductName = "Product 1",
                    Category = "Test Category 1"
                },
                new Product
                {
                    ProductID = Guid.NewGuid(),
                    ProductName = "Product 2",
                    Category = "Test Category 2"
                });

            await context.SaveChangesAsync();

            var repository = new ProductsRepository(context);

            // Act
            var result = await repository.GetProducts();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProductsByCondition_ShouldReturnMatchingProducts()
        {
            // Arrange
            using var context = GetDbContext();

            context.Products.AddRange(
                new Product
                {
                    ProductID = Guid.NewGuid(),
                    ProductName = "Apple",
                    Category = "Test Category"
                },
                new Product
                {
                    ProductID = Guid.NewGuid(),
                    ProductName = "Banana",
                    Category = "Test Category"
                });

            await context.SaveChangesAsync();

            var repository = new ProductsRepository(context);

            // Act
            var result = await repository.GetProductsByCondition(
                p => p.ProductName == "Apple");

            // Assert
            Assert.Single(result);
            Assert.Equal("Apple", result.First().ProductName);
        }

        [Fact]
        public async Task DeleteProduct_ShouldDeleteProduct()
        {
            // Arrange
            using var context = GetDbContext();

            var product = new Product
            {
                ProductID = Guid.NewGuid(),
                ProductName = "Test Product",
                Category = "Test Category"
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductsRepository(context);

            // Act
            var result = await repository.DeleteProduct(product.ProductID);

            // Assert
            Assert.True(result);

            var deletedProduct = await context.Products.FindAsync(product.ProductID);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            // Arrange
            using var context = GetDbContext();
            var repository = new ProductsRepository(context);

            // Act
            var result = await repository.DeleteProduct(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateProduct_ShouldUpdateExistingProduct()
        {
            // Arrange
            using var context = GetDbContext();

            var product = new Product
            {
                ProductID = Guid.NewGuid(),
                ProductName = "Original Product",
                Category = "Original Category"
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductsRepository(context);

            var updatedProduct = new Product
            {
                ProductID = product.ProductID,
                ProductName = "Updated Product",
                Category = "Updated Category"
            };

            // Act
            var result = await repository.UpdateProduct(updatedProduct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductID, result.ProductID);
            Assert.Equal("Updated Product", result.ProductName);
            Assert.Equal("Updated Category", result.Category);

            var savedProduct = await context.Products.FindAsync(product.ProductID);
            Assert.Equal("Updated Product", savedProduct.ProductName);
            Assert.Equal("Updated Category", savedProduct.Category);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            using var context = GetDbContext();
            var repository = new ProductsRepository(context);

            var product = new Product
            {
                ProductID = Guid.NewGuid(),
                ProductName = "Non Existing Product"
            };

            // Act
            var result = await repository.UpdateProduct(product);

            // Assert
            Assert.Null(result);
        }
    }
}