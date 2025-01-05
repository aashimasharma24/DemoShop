using System.ComponentModel.DataAnnotations;

namespace DemoShop.Core.DTOs
{
    public class OrderRequest
    {
        [Required(ErrorMessage = "Shipping address is required.")]
        public string ShippingAddress { get; set; } = null!;

        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; } = null!;
    }
}