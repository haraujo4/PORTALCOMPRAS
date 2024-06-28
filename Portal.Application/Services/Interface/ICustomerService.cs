using Portal.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Application.Services.Interface
{
    public interface ICustomerService
    {
        Task<CustomerDTO> GetCustomerByIdAsync(Guid id);
        Task<List<CustomerDTO>> GetAllCustomersAsync(int pageNumber, int pageSize);
        Task AddCustomerAsync(CreateCustomerDTO createCustomerDto);
        Task UpdateCustomerAsync(Guid id, UpdateCustomerDTO updateCustomerDto);
        Task DeleteCustomerAsync(Guid id);
        Task<string> LoginAsync(LoginDTO loginDto);
        Task<string> RecoverPasswordAsync(string email);
    }
}
