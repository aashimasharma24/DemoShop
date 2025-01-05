using Asp.Versioning;
using DemoShop.Core.DataObjects;
using DemoShop.Core.DTOs;
using DemoShop.Manager.Repositories.Interfaces;
using DemoShop.Manager.Services.Interfaces;
using Hangfire;
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
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;

        public OrdersController(
            ILogger<OrdersController> logger,
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IPaymentService paymentService,
            IEmailService emailService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _paymentService = paymentService;
            _emailService = emailService;
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
            _logger.LogInformation("Fetching order with id {OrderId}", orderId);

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found.", orderId);
                return NotFound();
            }

            // If the user is not Admin, ensure they can only see their own orders
            if (!IsAdmin() && order.UserId != GetUserId())
            {
                _logger.LogWarning("User {UserId} is not authorized to view order {OrderId}.", GetUserId(), orderId);
                return Forbid();
            }

            _logger.LogInformation("Order {OrderId} retrieved successfully.", orderId);
            return Ok(order);
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest request)
        {
            _logger.LogInformation("User {UserId} is placing an order", User?.FindFirst("userId")?.Value);

            var userId = GetUserId();

            // 1. Get cart items
            var cartItems = await _cartRepository.GetCartItemsAsync(userId);
            if (!cartItems.Any()) return BadRequest("Cart is empty.");

            // 2. Calculate total
            var totalAmount = cartItems.Sum(ci => ci.Product!.Price * ci.Quantity);

            // 3. Process Payment
            var paymentSuccess = await _paymentService.ProcessPayment(totalAmount, request.PaymentMethod, userId);
            if (!paymentSuccess) 
            {
                _logger.LogWarning("Payment failed for user {UserId}.", userId);
                return BadRequest("Payment failed.");
            }

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

            // 8. Enqueue Hangfire job
            BackgroundJob.Enqueue(() => _emailService.SendOrderConfirmationEmailAsync(userId, order.Id));


            _logger.LogInformation("Order {OrderId} placed successfully by user {UserId}.",
                order.Id,
                User?.FindFirst("userId")?.Value);

            return Ok(order);
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return NotFound($"Order #{orderId} not found.");

            // Only Admin can update status
            await _orderRepository.UpdateStatusAsync(orderId, status);

            return Ok($"Order #{orderId} status updated to '{status}'.");
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst("userId")!.Value);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }
    }
}
