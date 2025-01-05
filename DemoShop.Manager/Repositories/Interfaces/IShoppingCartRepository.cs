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
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int userId);
        Task AddOrUpdateCartItemAsync(int userId, int productId, int quantity);
        Task RemoveCartItemAsync(int cartItemId);
        Task ClearCartAsync(int userId);
    }
}
