using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;
using DemoShop.Manager.Services.Interfaces;
using Serilog;

namespace DemoShop.Manager.Services
{
    public class ProductService : IProductService
    {
        private readonly DemoShopDbContext _context;
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository, DemoShopDbContext context)
        {
            _repository = repository;
            _context = context;
            Log.Information("ProductService initialized.");
        }

        public IEnumerable<Product> GetAll()
        {
            return _repository.GetAll();
        }

        public Product GetByGUID(String guid)
        {
            return _repository.GetByGUID(guid);
        }

        public void Add(Product product)
        {
            _repository.Add(product);
            _context.SaveChanges();
        }

        public void Update(String guid, Product product)
        {
            var existingProduct = _repository.GetByGUID(guid);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.Category = product.Category;
                _repository.Update(existingProduct);
                _context.SaveChanges();
            }
        }

        public void Delete(String guid)
        {
            var product = _repository.GetByGUID(guid);
            if (product != null)
            {
                _repository.Delete(product);
                _context.SaveChanges();
            }
        }
    }
}