using DemoShop.Core.DataObjects;

namespace DemoShop.Manager.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);

        Task<IEnumerable<Product>> SearchAsync(string? name, string? category, decimal? minPrice, decimal? maxPrice);
    }
}
