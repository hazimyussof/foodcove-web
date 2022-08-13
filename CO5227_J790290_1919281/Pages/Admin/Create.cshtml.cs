using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CO5227_J790290_1919281.Data;
using Microsoft.AspNetCore.Authorization;

namespace CO5227_J790290_1919281.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private AppDbContext _database;

        [BindProperty]
        public Meal ML { get; set; }

        public CreateModel(AppDbContext database)
        {
            _database = database;
        }

        public void OnGet()
        {
        }

        // Create Page's Submit button method
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) { return Page(); } // Validation for submit button.
            ML.Active = true; // When adding a new data, that data will automatically be set to true.

            foreach (var file in Request.Form.Files)
            {
                MemoryStream ms = new MemoryStream();
                file.CopyTo(ms);
                ML.ImageData = ms.ToArray();

                ms.Close();
                ms.Dispose();
            }

            _database.Meals.Add(ML);
            await _database.SaveChangesAsync();
            return RedirectToPage("/Admin/Create"); // Refreshing page after submiting request.
        }

    }
}
