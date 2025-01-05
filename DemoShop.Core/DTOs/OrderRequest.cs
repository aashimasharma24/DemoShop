namespace DemoShop.Core.DTOs
{
    public class OrderRequest
    {
        public string ShippingAddress { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }
}