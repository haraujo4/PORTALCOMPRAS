using AutoMapper;
using Portal.Application.DTO;
using Portal.Application.Services.Interface;
using Portal.Domain.Entities;
using Portal.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Portal.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;

        }

        public async Task<IEnumerable<CartDTO>> GetCartByIdAsync(Guid id)
        {
            var carts = await _cartRepository.GetByIdAsync(id);
            if (carts == null || !carts.Any()) throw new Exception("Cart not found");

            return carts.Select(cart => new CartDTO
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                Items = cart.Items?.Select(item => new CartItemDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList() ?? new List<CartItemDTO>(),
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt
            });
        }

        public async Task<IEnumerable<CartDTO>> GetAllCartsAsync(Guid customerId, int pageNumber, int pageSize)
        {
            var carts = await _cartRepository.GetAllAsync(customerId, pageNumber, pageSize);
            return carts.Select(c => new CartDTO
            {
                Id = c.Id,
                CustomerId = c.CustomerId,
                Items = c.Items.Select(item => new CartItemDTO
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task AddCartItemAsync(Guid customerId, List<AddCartItemDTO> addCartItemDto)
        {
            var cart = await GetOrCreateCartAsync(customerId);
            var product = await VerifyProductExistsAsync(addCartItemDto.Select(c => c.ProductId).First());
            AddItemToCart(cart, addCartItemDto, product.Price);
            await SaveCartAsync(cart, customerId);
        }

        public async Task UpdateCartItemAsync(Guid cartId, Guid productId, UpdateCartItemDTO updateCartItemDto)
        {
            var carts = await _cartRepository.GetByIdAsync(cartId);
            var cart = carts.FirstOrDefault();
            if (cart == null) throw new Exception("Cart not found");

            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem == null) throw new Exception("Cart item not found");

            cartItem.Quantity = updateCartItemDto.Quantity;
            cartItem.Price = updateCartItemDto.Quantity * cartItem.Price;

            await _cartRepository.UpdateAsync(cart);
        }

        public async Task RemoveCartItemAsync(Guid cartId, Guid productId)
        {
            var carts = await _cartRepository.GetByIdAsync(cartId);
            var cart = carts.FirstOrDefault();
            if (cart == null) throw new Exception("Cart not found");

            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem == null) throw new Exception("Cart item not found");

            cart.Items.Remove(cartItem);

            await _cartRepository.UpdateAsync(cart);
        }

        public async Task<decimal> GetCartTotalAsync(Guid cartId)
        {
            var carts = await _cartRepository.GetByIdAsync(cartId);
            var cart = carts.FirstOrDefault();
            if (cart == null) throw new Exception("Cart not found");

            return cart.Items.Sum(item => item.Price);
        }

        private async Task<Cart> GetOrCreateCartAsync(Guid customerId)
        {
            var carts = await _cartRepository.GetAllAsync(customerId, 1, 1);
            var cart = carts.FirstOrDefault();

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Items = new List<CartItem>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            return cart;
        }

        private async Task<Product> VerifyProductExistsAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            return product;
        }

        private void AddItemToCart(Cart cart, List<AddCartItemDTO> addCartItemDtos, decimal productPrice)
        {
            foreach (var addCartItemDto in addCartItemDtos)
            {
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = addCartItemDto.ProductId,
                    Quantity = addCartItemDto.Quantity,
                    Price = productPrice * addCartItemDto.Quantity
                };
                cart.Items.Add(cartItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
        }

        private async Task SaveCartAsync(Cart cart, Guid customerId)
        {
            var cartExist = await _cartRepository.GetByIdAsync(customerId);
            if (!cartExist.Any())
            {
                await _cartRepository.AddAsync(cart);
            }
            else
            {
                throw new Exception("Cart already exists");
            }
        }

        private async Task UpdateCustomerWithCartAsync(Guid customerId, Cart cart)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer != null)
            {
                throw new Exception("Customer not found");
            }

            if (customer.Carts == null)
            {
                customer.Carts = new List<Cart> { cart };
            }
            else
            {
                var existingCart = customer.Carts.FirstOrDefault(c => c.Id == cart.Id);
                if (existingCart == null)
                {
                    customer.Carts.Add(cart);
                }
                else
                {
                    existingCart.Items = cart.Items;
                    existingCart.UpdatedAt = cart.UpdatedAt;
                }
            }
            await _customerRepository.UpdateAsync(customer);
        }
    }

}
