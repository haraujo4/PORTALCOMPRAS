using Portal.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Application.Services.Interface
{
    public interface ICartService
    {
        Task<IEnumerable<CartDTO>> GetCartByIdAsync(Guid id);
        Task<IEnumerable<CartDTO>> GetAllCartsAsync(Guid customerId, int pageNumber, int pageSize);
        Task AddCartItemAsync(Guid customerId, List<AddCartItemDTO> addCartItemDto);
        Task UpdateCartItemAsync(Guid cartId, Guid productId, UpdateCartItemDTO updateCartItemDto);
        Task RemoveCartItemAsync(Guid cartId, Guid productId);
        Task<decimal> GetCartTotalAsync(Guid cartId);
    }
}
