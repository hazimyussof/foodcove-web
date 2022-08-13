using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CO5227_J790290_1919281.Data;
using Microsoft.AspNetCore.Authorization;

namespace CO5227_J790290_1919281.Pages.Admin
{
    [Authorize]
    public class MenuModel : PageModel
    {
        private readonly AppDbContext _database;
        public IList<Meal> Meal { get; private set; }

        [BindProperty]
        public string Search { get; set; }

        public MenuModel(AppDbContext database)
        {
            _database = database;
        }

        public void OnGet()
        {
            Meal = _database.Meals.FromSqlRaw("SELECT * FROM Meals ORDER BY Active DESC").ToList();
        }

        public IActionResult OnPostSearch()
        {
            Meal = _database.Meals
                .FromSqlRaw("SELECT * FROM Meals WHERE Name LIKE '" + Search + "%' ORDER BY Active DESC")
                .ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int itemID)
        {
            var item = await _database.Meals.FindAsync(itemID);
            item.Active = false;//set item as deleted
            _database.Attach(item).State = EntityState.Modified;

            try
            {
                await _database.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new Exception($"Item {item.ID} not found!", e);
            }
            return RedirectToPage();

        }

    }
}
