using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly DemoShopDbContext _context;

        public ShoppingCartRepository(DemoShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ShoppingCartItem> GetCartItems(string userId) =>
            _context.ShoppingCartItems.Where(c => c.UserId == userId).ToList();

        public void AddToCart(ShoppingCartItem item)
        {
            _context.ShoppingCartItems.Add(item);
            _context.SaveChanges();
        }

        public void UpdateCartItem(ShoppingCartItem item)
        {
            if (item.Quantity == 0)
            {
                RemoveFromCart(item.Guid);
            }
            else
            {
                _context.ShoppingCartItems.Update(item);
                _context.SaveChanges();
            }
        }

        public void RemoveFromCart(String guid)
        {
            var item = _context.ShoppingCartItems.FirstOrDefault(x => x.Guid == guid);
            if (item != null)
            {
                _context.ShoppingCartItems.Remove(item);
                _context.SaveChanges();
            }
        }
        public void ClearCart(string userId)
        {
            var items = _context.ShoppingCartItems.Where(c => c.UserId == userId).ToList();
            _context.ShoppingCartItems.RemoveRange(items);
            _context.SaveChanges();
        }

        public decimal CalculateCartTotal(string userId)
        {
            return _context.ShoppingCartItems
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity * c.Price);
        }
    }
}
