using DemoShop.Core.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Repositories.Interfaces
{
    public interface IShoppingCartRepository
    {
        IEnumerable<ShoppingCartItem> GetCartItems(string userId);
        void AddToCart(ShoppingCartItem item);
        void UpdateCartItem(ShoppingCartItem item);
        void RemoveFromCart(String guid);
        void ClearCart(string userId);
        decimal CalculateCartTotal(string userId);
    }
}
