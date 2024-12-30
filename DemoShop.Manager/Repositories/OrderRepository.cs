using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;

namespace DemoShop.Manager.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DemoShopDbContext _context;

        public OrderRepository(DemoShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAll()
        {
           return _context.Orders.ToList();
        }

        public Order GetByGUID(String guid)
        {

           var res = _context.Orders.FirstOrDefault(x => x.Guid == guid);
            return res;
        }

        public void Add(Order order) 
        { 
            _context.Orders.Add(order); 
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Delete(Order order)
        {
            _context.Orders.Remove(order);
        }

    }
}
