using DemoShop.Core.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services.Interfaces
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAll();
        Order GetByGUID(String guid);
        void Add(Order order);
        void Update(String guid, Order order);
        void Delete(String guid);
    }
}
