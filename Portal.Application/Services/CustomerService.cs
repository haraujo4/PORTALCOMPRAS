using AutoMapper;
using Portal.Application.DTO;
using Portal.Application.Helpers;
using Portal.Application.Services.Interface;
using Portal.Domain.Entities;
using Portal.Domain.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Application.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly JwtService _jwtService;

        public CustomerService(ICustomerRepository customerRepository, JwtService jwtHeader)
        {
            _customerRepository = customerRepository;
            _jwtService = jwtHeader;
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                PasswordHash = customer.PasswordHash,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
                Carts = customer.Carts
            };
        }

        public async Task<List<CustomerDTO>> GetAllCustomersAsync(int pageNumber, int pageSize)
        {
            var customers = await _customerRepository.GetAllAsync(pageNumber, pageSize);
            return customers.Select(c => new CustomerDTO
            {
                Id = c.Id,
                Name = c.Name,
                PasswordHash = c.PasswordHash,
                Carts = c.Carts,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                
            }).ToList();
        }

        public async Task AddCustomerAsync(CreateCustomerDTO createCustomerDto)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = createCustomerDto.Name,
                Email = createCustomerDto.Email,
                PasswordHash = PasswordHashHelpers.HashPassword(createCustomerDto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(Guid id, UpdateCustomerDTO updateCustomerDto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) throw new Exception("Customer not found");

            customer.Name = updateCustomerDto.Name;
            customer.Email = updateCustomerDto.Email;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(Guid id)
        {
            await _customerRepository.DeleteAsync(id);
        }

        //logar

        public async Task<string> LoginAsync(LoginDTO loginDto)
        {
            var customer = await _customerRepository.GetByEmailAsync(loginDto.Email);
            if (customer == null) throw new Exception("Invalid email or password");

            if (!PasswordHashHelpers.VerifyPassword(loginDto.Password, customer.PasswordHash))
                throw new Exception("Invalid email or password");

            var token = _jwtService.GenerateToken(customer.Id.ToString(), customer.Email);
            return token;
        }

        //recuperar senha

        public async Task<string> RecoverPasswordAsync(string email)
        {
            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null) throw new Exception("Invalid email");

            //enviar email com nova senha
            var newPassword = PasswordHashHelpers.GenerateRandomPassword();
            customer.PasswordHash = PasswordHashHelpers.HashPassword(newPassword);
            await _customerRepository.UpdateAsync(customer);
            EmailHelpers.SendPasswordResetEmailAsync(customer.Email, newPassword);

            return "Password reset email sent";
        }

    }
}
