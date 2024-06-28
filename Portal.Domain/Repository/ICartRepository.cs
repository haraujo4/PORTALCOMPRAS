using Portal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Domain.Repository
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetByIdAsync(Guid id);
        Task<IEnumerable<Cart>> GetAllAsync(Guid customerId, int pageNumber, int pageSize);
        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Guid id);
    }
}
