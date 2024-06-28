using Microsoft.EntityFrameworkCore;
using Portal.Domain.Entities;
using Portal.Domain.Repository;
using Portal.Infra.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Infra.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _dbContext;

        public ProductRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
