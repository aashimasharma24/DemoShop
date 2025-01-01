using DemoShop.Core.DataObjects;
using DemoShop.Manager.Repositories.Interfaces;
using DemoShop.Manager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _repository;

        public ShoppingCartService(IShoppingCartRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ShoppingCartItem> GetCartItems(string userId) =>
            _repository.GetCartItems(userId);

        public void AddToCart(ShoppingCartItem item)
        {
            if (item.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }
            _repository.AddToCart(item);
        }

        public void UpdateCartItem(ShoppingCartItem item)
        {
            if (item.Quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.");
            }
            _repository.UpdateCartItem(item);
        }

        public void RemoveFromCart(String guid) =>
            _repository.RemoveFromCart(guid);

        public void ClearCart(String userId) =>
            _repository.ClearCart(userId);

        public decimal CalculateCartTotal(String userId) =>
            _repository.CalculateCartTotal(userId);
    }
}
