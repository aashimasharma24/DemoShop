using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Core.DataObjects
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public required string Guid { get; set; }
        [DataType(DataType.Date)] 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
