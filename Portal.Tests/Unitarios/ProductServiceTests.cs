using AutoMapper;
using Moq;
using Portal.Application.DTO;
using Portal.Application.Services;
using Portal.Domain.Entities;
using Portal.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Tests.Unitarios
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
        }

        [Fact]
        public async Task GetProductByIdAsync_ValidId_ReturnsProductDto()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productService = new ProductService(_mockProductRepository.Object);
            var product = new Product { Id = productId, Name = "Test Product", Price = 10.0m };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await productService.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Price, result.Price);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsListOfProductDtos()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var productService = new ProductService(_mockProductRepository.Object);
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product 1", Price = 10.0m },
                new Product { Id = Guid.NewGuid(), Name = "Product 2", Price = 15.0m }
            };
            _mockProductRepository.Setup(repo => repo.GetAllAsync(pageNumber, pageSize)).ReturnsAsync(products);

            // Act
            var result = await productService.GetAllProductsAsync(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(products.Count, result.Count());
            Assert.Equal(products[0].Name, result.ElementAt(0).Name);
            Assert.Equal(products[1].Price, result.ElementAt(1).Price);
        }

        [Fact]
        public async Task AddProductAsync_ValidProductDto_AddsProductToRepository()
        {
            // Arrange
            var productService = new ProductService(_mockProductRepository.Object);
            var createProductDto = new CreateProductDTO
            {
                Name = "New Product",
                Description = "Description",
                Price = 20.0m
            };

            // Act
            await productService.AddProductAsync(createProductDto);

            // Assert
            _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ValidIdAndProductDto_UpdatesProductInRepository()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productService = new ProductService(_mockProductRepository.Object);
            var updateProductDto = new UpdateProductDTO
            {
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 25.0m
            };
            var existingProduct = new Product { Id = productId, Name = "Old Product", Price = 20.0m };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(existingProduct);

            // Act
            await productService.UpdateProductAsync(productId, updateProductDto);

            // Assert
            _mockProductRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
            Assert.Equal(updateProductDto.Name, existingProduct.Name);
            Assert.Equal(updateProductDto.Price, existingProduct.Price);
        }

        [Fact]
        public async Task DeleteProductAsync_ValidId_RemovesProductFromRepository()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productService = new ProductService(_mockProductRepository.Object);

            // Act
            await productService.DeleteProductAsync(productId);

            // Assert
            _mockProductRepository.Verify(repo => repo.DeleteAsync(productId), Times.Once);
        }
    }
}
