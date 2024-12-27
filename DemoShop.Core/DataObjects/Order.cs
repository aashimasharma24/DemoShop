using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Core.DataObjects
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<OrderItem> Items { get; set; }
        public string Status { get; set; }
        public required string Guid { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

    }
}
