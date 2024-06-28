using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Application.DTO;
using Portal.Application.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar cliente: {ex.Message}");
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync(pageNumber, pageSize);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar clientes: {ex.Message}");
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateCustomerDTO createCustomerDto)
        {
            try
            {
                await _customerService.AddCustomerAsync(createCustomerDto);
                return Ok("Cliente registrado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao registrar cliente: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerDTO updateCustomerDto)
        {
            try
            {
                await _customerService.UpdateCustomerAsync(id, updateCustomerDto);
                return Ok("Cliente atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao atualizar cliente: {ex.Message}");
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(id);
                return Ok("Cliente deletado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao deletar cliente: {ex.Message}");
            }
        }
    }
}
