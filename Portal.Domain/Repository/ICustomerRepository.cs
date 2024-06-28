using Portal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Domain.Repository
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(Guid id);
        Task<List<Customer>> GetAllAsync(int pageNumber, int pageSize);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Guid id);
        Task<Customer> GetByEmailAsync(string email);
    }
}
