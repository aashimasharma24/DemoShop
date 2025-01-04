using DemoShop.Core.DataObjects;
using DemoShop.Manager.Repositories.Interfaces;
using DemoShop.Manager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IEnumerable<Category> GetAllCategories() => _categoryRepository.GetAll();
        public Category GetCategoryById(int id) => _categoryRepository.GetById(id);
        public void AddCategory(Category category) => _categoryRepository.Add(category);
        public void UpdateCategory(int id, Category category)
        {
            var existingCategory = _categoryRepository.GetById(id);
            if (existingCategory == null)
            {
                throw new InvalidOperationException("Category not found.");
            }
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            _categoryRepository.Update(existingCategory);
        }
        public void DeleteCategory(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                throw new InvalidOperationException("Category not found.");
            }
            _categoryRepository.Delete(category);
        }
    }
}
