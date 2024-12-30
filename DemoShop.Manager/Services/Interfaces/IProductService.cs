using DemoShop.Core.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services.Interfaces
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        Product GetByGUID(String guid);
        void Add(Product product);
        void Update(String guid, Product product);
        void Delete(String guid);
    }
}
