using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;
using DemoShop.Manager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly DemoShopDbContext _context;

        public OrderService(IOrderRepository repository, DemoShopDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public IEnumerable<Order> GetAll()
        {
            return _repository.GetAll();
        }

        public Order GetByGUID(String guid)
        {
            return _repository.GetByGUID(guid);
        }

        public void Add(Order order)
        {
            _repository.Add(order);
            _context.SaveChanges();
        }

        public void Update(String guid, Order order)
        {
            var existingOrder = _repository.GetByGUID(guid);
            if (existingOrder != null)
            {
                _repository.Update(order);
                _context.SaveChanges();
            }
        }

        public void Delete(String guid)
        {
            var order = _repository.GetByGUID(guid);
            if (order != null)
            {
                _repository.Delete(order);
                _context.SaveChanges();
            }
        }

    }
}
