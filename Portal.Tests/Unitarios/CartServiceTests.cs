using AutoMapper;
using Moq;
using Portal.Application.DTO;
using Portal.Application.Services;
using Portal.Domain.Entities;
using Portal.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Tests.Unitarios
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;

        public CartServiceTests()
        {
            _mockCartRepository = new Mock<ICartRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();

            
        }

        [Fact]
        public async Task GetCartByIdAsync_ReturnsCorrectCart()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            var cartEntity = new Cart
            {
                Id = cartId,
                CustomerId = customerId,
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = Guid.NewGuid(), Quantity = 1, Price = 10 },
                    new CartItem { ProductId = Guid.NewGuid(), Quantity = 2, Price = 20 }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                               .ReturnsAsync(new List<Cart> { cartEntity });

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act
            var result = await cartService.GetCartByIdAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartId, result.First().Id);
            Assert.Equal(customerId, result.First().CustomerId);
            Assert.Equal(cartEntity.Items.Count, result.First().Items.Count());
        }

        [Fact]
        public async Task GetCartByIdAsync_CartNotFound_ThrowsException()
        {
            // Arrange
            var cartId = Guid.NewGuid();

            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                               .ReturnsAsync(new List<Cart>());

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await cartService.GetCartByIdAsync(cartId));
        }
        [Fact]
        public async Task GetAllCartsAsync_ReturnsCorrectCarts()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 10;

            var cartEntities = new List<Cart>
            {
                new Cart
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Items = new List<CartItem>
                    {
                        new CartItem { ProductId = Guid.NewGuid(), Quantity = 1, Price = 10 },
                        new CartItem { ProductId = Guid.NewGuid(), Quantity = 2, Price = 20 }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Cart
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Items = new List<CartItem>
                    {
                        new CartItem { ProductId = Guid.NewGuid(), Quantity = 3, Price = 30 },
                        new CartItem { ProductId = Guid.NewGuid(), Quantity = 4, Price = 40 }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _mockCartRepository.Setup(repo => repo.GetAllAsync(customerId, pageNumber, pageSize))
                               .ReturnsAsync(cartEntities);

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act
            var result = await cartService.GetAllCartsAsync(customerId, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartEntities.Count, result.Count());
            Assert.Equal(cartEntities.First().Items.Count, result.First().Items.Count());
            Assert.Equal(cartEntities.Last().Items.Count, result.Last().Items.Count);
        }

        [Fact]
        public async Task GetAllCartsAsync_NoCartsFound_ReturnsEmptyList()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 10;

            _mockCartRepository.Setup(repo => repo.GetAllAsync(customerId, pageNumber, pageSize))
                               .ReturnsAsync(new List<Cart>());

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act
            var result = await cartService.GetAllCartsAsync(customerId, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Fact]
        public async Task AddCartItemAsync_SuccessfullyAddsItemToCart()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var addCartItemDto = new List<AddCartItemDTO>
    {
        new AddCartItemDTO { ProductId = productId, Quantity = 2 }
    };

            var cartEntity = new Cart
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Items = new List<CartItem>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var productEntity = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 50
            };

            _mockCartRepository.Setup(repo => repo.GetByIdAsync(customerId))
                               .ReturnsAsync(new List<Cart>());

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                                  .ReturnsAsync(productEntity);

            _mockCartRepository.Setup(repo => repo.AddAsync(It.IsAny<Cart>()))
                               .Returns(Task.CompletedTask)
                               .Callback((Cart cart) =>
                               {
                                   foreach (var dto in addCartItemDto)
                                   {
                                       var cartItem = new CartItem
                                       {
                                           Id = Guid.NewGuid(),
                                           CartId = cart.Id,
                                           ProductId = dto.ProductId,
                                           Quantity = dto.Quantity,
                                           Price = dto.Quantity * productEntity.Price
                                       };
                                       cartEntity.Items.Add(cartItem);
                                   }
                               });

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act
            await cartService.AddCartItemAsync(customerId, addCartItemDto);

            // Assert
            Assert.Single(cartEntity.Items);
            var addedItem = cartEntity.Items.First();
            Assert.Equal(productId, addedItem.ProductId);
            Assert.Equal(2, addedItem.Quantity);
            Assert.Equal(100, addedItem.Price);
        }
        [Fact]
        public async Task AddCartItemAsync_ProductNotFound_ThrowsException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var addCartItemDto = new List<AddCartItemDTO>
    {
        new AddCartItemDTO { ProductId = productId, Quantity = 2 }
    };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId))
                                  .ReturnsAsync((Product)null);

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await cartService.AddCartItemAsync(customerId, addCartItemDto));
        }
        [Fact]
        public async Task UpdateCartItemAsync_SuccessfullyUpdatesCartItem()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateCartItemDto = new UpdateCartItemDTO { Quantity = 3 };

            var cartEntity = new Cart
            {
                Id = cartId,
                CustomerId = Guid.NewGuid(),
                Items = new List<CartItem>
        {
            new CartItem { Id = Guid.NewGuid(), ProductId = productId, Quantity = 2, Price = 100 }
        },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                               .ReturnsAsync(new List<Cart> { cartEntity });

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act
            await cartService.UpdateCartItemAsync(cartId, productId, updateCartItemDto);

            // Assert
            var updatedItem = cartEntity.Items.First();
            Assert.Equal(3, updatedItem.Quantity);
            Assert.Equal(300, updatedItem.Price); 
        }

        [Fact]
        public async Task UpdateCartItemAsync_CartOrItemNotFound_ThrowsException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var updateCartItemDto = new UpdateCartItemDTO { Quantity = 3 };

            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                               .ReturnsAsync(new List<Cart>());

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await cartService.UpdateCartItemAsync(cartId, productId, updateCartItemDto));
        }
        [Fact]
        public async Task RemoveCartItemAsync_SuccessfullyRemovesCartItem()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var cartEntity = new Cart
            {
                Id = cartId,
                CustomerId = Guid.NewGuid(),
                Items = new List<CartItem>
        {
            new CartItem { Id = Guid.NewGuid(), ProductId = productId, Quantity = 2, Price = 100 }
        },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                               .ReturnsAsync(new List<Cart> { cartEntity });

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act
            await cartService.RemoveCartItemAsync(cartId, productId);

            // Assert
            Assert.Empty(cartEntity.Items);
        }

        [Fact]
        public async Task RemoveCartItemAsync_CartOrItemNotFound_ThrowsException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                               .ReturnsAsync(new List<Cart>());

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await cartService.RemoveCartItemAsync(cartId, productId));
        }

        [Fact]
        public async Task GetCartTotalAsync_CartNotFound_ThrowsException()
        {
            // Arrange
            var cartId = Guid.NewGuid();

            _mockCartRepository.Setup(repo => repo.GetByIdAsync(cartId))
                               .ReturnsAsync(new List<Cart>());

            var cartService = new CartService(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockCustomerRepository.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await cartService.GetCartTotalAsync(cartId));
        }

    }
}
