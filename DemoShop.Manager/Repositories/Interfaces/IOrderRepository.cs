using DemoShop.Core.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll();
        Order GetByGUID(String guid);
        void Add(Order order);
        void Update(Order order);
        void Delete(Order order);
    }
}
