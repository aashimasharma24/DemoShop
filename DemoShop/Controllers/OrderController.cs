using Asp.Versioning;
using DemoShop.Core.DataObjects;
using DemoShop.Core.DTOs;
using DemoShop.Manager.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaymentService _paymentService;

        public OrdersController(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IPaymentService paymentService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _paymentService = paymentService;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = GetUserId();
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return NotFound();

            // Optionally check user identity if not admin
            return Ok(order);
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest request)
        {
            var userId = GetUserId();

            // 1. Get cart items
            var cartItems = await _cartRepository.GetCartItemsAsync(userId);
            if (!cartItems.Any()) return BadRequest("Cart is empty.");

            // 2. Calculate total
            var totalAmount = cartItems.Sum(ci => ci.Product!.Price * ci.Quantity);

            // 3. Process Payment
            var paymentSuccess = await _paymentService.ProcessPayment(totalAmount, request.PaymentMethod, userId);
            if (!paymentSuccess) return BadRequest("Payment failed.");

            // 4. Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                ShippingAddress = request.ShippingAddress,
                PaymentMethod = request.PaymentMethod,
                Status = "Pending"
            };

            // 5. Create OrderItems
            foreach (var ci in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product!.Price
                });

                // Deduct stock
                ci.Product.StockCount -= ci.Quantity;
                await _productRepository.UpdateAsync(ci.Product);
            }

            // 6. Save order
            await _orderRepository.AddAsync(order);

            // 7. Clear cart
            await _cartRepository.ClearCartAsync(userId);

            // In real scenario, you might fire an async task to send a confirmation email
            return Ok(order);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst("userId")!.Value);
        }
    }
}
