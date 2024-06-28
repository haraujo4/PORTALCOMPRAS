using Microsoft.AspNetCore.Mvc;
using Portal.Application.DTO;
using Portal.Application.Services;
using Portal.Application.Services.Interface;
using Portal.Domain.Repository;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticatorController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public AuthenticatorController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var token = await _customerService.LoginAsync(loginDto);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = $"Falha ao autenticar: {ex.Message}" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                var result = await _customerService.RecoverPasswordAsync(email);
                return Ok($"Um e-mail de recuperação de senha foi enviado para {email}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao processar solicitação: {ex.Message}");
            }
        }
    }
}
