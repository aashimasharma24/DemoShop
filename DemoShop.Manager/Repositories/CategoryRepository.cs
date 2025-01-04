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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DemoShopDbContext _context;

        public CategoryRepository(DemoShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAll() => _context.Categories.ToList();
        public Category GetById(int id) => _context.Categories.Find(id);
        public void Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }
        public void Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
    }
}
