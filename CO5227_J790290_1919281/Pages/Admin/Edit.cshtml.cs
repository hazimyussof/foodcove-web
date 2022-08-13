using CO5227_J790290_1919281.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CO5227_J790290_1919281.Pages.Admin
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public Meal Item { get; set; }
        private readonly AppDbContext _database;

        public EditModel(AppDbContext database) 
        { 
            _database = database; 
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _database.Meals.FindAsync(id);
            if (Item == null)
            {
                return RedirectToPage("/Admin/Menu");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _database.Attach(Item).State = EntityState.Modified;

            try
            {
                await _database.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new Exception($"Item {Item.ID} not found!", e);
            }
            return RedirectToPage("/Admin/Menu");
        }

    }

}
