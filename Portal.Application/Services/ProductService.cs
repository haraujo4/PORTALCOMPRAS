using AutoMapper;
using Portal.Application.DTO;
using Portal.Application.Services.Interface;
using Portal.Domain.Entities;
using Portal.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Application.Services
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDTO> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            var products = await _productRepository.GetAllAsync(pageNumber, pageSize);
            return products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task AddProductAsync(CreateProductDTO createProductDto)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Guid id, UpdateProductDTO updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}
