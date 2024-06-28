using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portal.Application.DTO;

using Portal.Application.Services.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar produto: {ex.Message}");
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync(pageNumber, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar produtos: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDTO createProductDto)
        {
            try
            {
                await _productService.AddProductAsync(createProductDto);
                return Ok("Produto adicionado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao adicionar produto: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDTO updateProductDto)
        {
            try
            {
                await _productService.UpdateProductAsync(id, updateProductDto);
                return Ok("Produto atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao atualizar produto: {ex.Message}");
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return Ok("Produto deletado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Falha ao deletar produto: {ex.Message}");
            }
        }
    }
}
