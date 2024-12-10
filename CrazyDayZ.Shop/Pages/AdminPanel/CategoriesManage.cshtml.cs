using CrazyDayZ.Shop.Data;
using CrazyDayZ.Shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrazyDayZ.Shop.Pages.AdminPanel
{
    public class CategoriesManageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CategoriesManageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; }
        public IList<Category> Categories { get; set; }

        public async Task OnGetAsync()
        {
            Category = new Category();
            Categories = await _context.Categories.OrderBy(c => c.Order).ToListAsync();
        }


        public async Task<IActionResult> OnPostSaveCategoryAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Category.Id == 0) 
            {
                var maxOrder = await _context.Categories.MaxAsync(c => (int?)c.Order) ?? 0;
                Category.Order = maxOrder + 1; 
                _context.Categories.Add(Category);
            }
            else 
            {
                var existingCategory = await _context.Categories.FindAsync(Category.Id);
                if (existingCategory != null)
                {
                    existingCategory.Name = Category.Name; 
                }
            }

            await _context.SaveChangesAsync();
            return Partial("_CategoriesTable", await _context.Categories.OrderBy(c => c.Order).ToListAsync()); 
        }


        public async Task<IActionResult> OnGetGetCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return new JsonResult(new { id = category.Id, name = category.Name });
        }

        public async Task<IActionResult> OnPostDeleteCategoryAsync(int id)
        {
            Console.WriteLine($"Попытка удалить категорию с ID: {id}");
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                Console.WriteLine("Категория не найдена.");
                return BadRequest("Категория не найдена.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            Console.WriteLine("Категория успешно удалена.");

            return Partial("_CategoriesTable", await _context.Categories.ToListAsync());
        }

        public async Task<IActionResult> OnPostUpdateCategoryOrderAsync(string[] order)
        {
            if (order == null || order.Length == 0)
            {
                return BadRequest("Не получен порядок категорий.");
            }

            for (int i = 0; i < order.Length; i++)
            {
                int categoryId = int.Parse(order[i]);
                var category = await _context.Categories.FindAsync(categoryId);
                if (category != null)
                {
                    category.Order = i + 1; 
                }
            }

            await _context.SaveChangesAsync();
            return Partial("_CategoriesTable", await _context.Categories.OrderBy(c => c.Order).ToListAsync());
        }
    }
}
