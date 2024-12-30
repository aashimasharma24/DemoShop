using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;

namespace DemoShop.Manager.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DemoShopDbContext _context;

        public ProductRepository(DemoShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public Product GetByGUID(String guid)
        {
            return _context.Products.FirstOrDefault(x => x.Guid == guid);
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
        }
    }
}
