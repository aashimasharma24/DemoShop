using DemoShop.Core.DataObjects;

namespace DemoShop.Manager.Repositories.Interfaces
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product GetByGUID(String guid);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}
