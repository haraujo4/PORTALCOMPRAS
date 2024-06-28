using Portal.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Application.Services.Interface
{
    public interface IProductService
    {
        Task<ProductDTO> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync(int pageNumber, int pageSize);
        Task AddProductAsync(CreateProductDTO createProductDto);
        Task UpdateProductAsync(Guid id, UpdateProductDTO updateProductDto);
        Task DeleteProductAsync(Guid id);

    }
}
