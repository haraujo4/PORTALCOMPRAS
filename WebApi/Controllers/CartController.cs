using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Application.DTO;
using Portal.Application.Services.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCart(Guid customerId)
        {
            try
            {
                var cart = await _cartService.GetCartByIdAsync(customerId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar carrinho: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPost("{customerId}/add-item")]
        public async Task<IActionResult> AddCartItem(Guid customerId, [FromBody] List<AddCartItemDTO> addCartItemDto)
        {
            try
            {
                await _cartService.AddCartItemAsync(customerId, addCartItemDto);
                return Ok("Item adicionado ao carrinho com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao adicionar item ao carrinho: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPut("{customerId}/update-item/{productId}")]
        public async Task<IActionResult> UpdateCartItem(Guid customerId, Guid productId, [FromBody] UpdateCartItemDTO updateCartItemDto)
        {
            try
            {
                await _cartService.UpdateCartItemAsync(customerId, productId, updateCartItemDto);
                return Ok("Item do carrinho atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao atualizar item do carrinho: {ex.Message}");
            }
        }
        [Authorize]
        [HttpDelete("{customerId}/remove-item/{productId}")]
        public async Task<IActionResult> RemoveCartItem(Guid customerId, Guid productId)
        {
            try
            {
                await _cartService.RemoveCartItemAsync(customerId, productId);
                return Ok("Item removido do carrinho com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao remover item do carrinho: {ex.Message}");
            }
        }
    }
}
