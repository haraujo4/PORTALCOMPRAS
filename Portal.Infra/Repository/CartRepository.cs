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
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _dbContext;

        public CartRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Cart>> GetByIdAsync(Guid id)
        {
            return await _dbContext.Carts
                .Where(c => c.CustomerId == id)
                .Include(c => c.Items)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cart>> GetAllAsync(Guid customerId, int pageNumber, int pageSize)
        {
            return await _dbContext.Carts
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Items) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(Cart cart)
        {
            await _dbContext.Carts.AddAsync(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _dbContext.Carts.Update(cart);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var cart = await GetAllAsync(id, 1, 1);
            if (cart.Any())
            {
                _dbContext.Carts.Remove(cart.First());
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
