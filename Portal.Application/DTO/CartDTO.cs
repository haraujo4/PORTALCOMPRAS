using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Application.DTO
{
    public class CartDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public ICollection<CartItemDTO> Items { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItemDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class AddCartItemDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDTO
    {
        public int Quantity { get; set; }
    }
}
