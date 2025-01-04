using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DemoShop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult GetAllCategories() => Ok(_categoryService.GetAllCategories());

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCategory([FromBody] Category category)
        {
            _categoryService.AddCategory(category);
            return Ok(new { message = "Category added successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            _categoryService.UpdateCategory(id, category);
            return Ok(new { message = "Category updated successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            _categoryService.DeleteCategory(id);
            return Ok(new { message = "Category deleted successfully." });
        }
    }
}
