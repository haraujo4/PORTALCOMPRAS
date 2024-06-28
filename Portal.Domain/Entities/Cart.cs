using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public ICollection<CartItem> Items { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
