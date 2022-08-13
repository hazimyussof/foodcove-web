using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CO5227_J790290_1919281.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CO5227_J790290_1919281.Pages
{
    public class MenuModel : PageModel
    {
        [BindProperty]
        public string Search { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly AppDbContext _database;
        public IList<Meal> Meal { get; private set; }

        public MenuModel(AppDbContext database, UserManager<ApplicationUser> userManager)
        {
            _database = database;
            _userManager = userManager;
        }

        public void OnGet()
        {
            Meal = _database.Meals.FromSqlRaw("SELECT * FROM Meals WHERE Active = 1").ToList();
        }

        public IActionResult OnPostSearch()
        {
            Meal = _database.Meals.FromSqlRaw("SELECT * FROM Meals WHERE Name LIKE '" + Search + "%' AND Active = 1").ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostBuyAsync(int itemID)
        {
            var user = await _userManager.GetUserAsync(User);
            CheckoutCustomer customer = await _database
            .CheckoutCustomers
            .FindAsync(user.Email);

            var item = _database.BasketItems.FromSqlRaw("SELECT * FROM BasketItems WHERE StockID = {0}" + " AND BasketID = {1}", itemID, customer.BasketID)
                        .ToList()
                        .FirstOrDefault();

            if (item == null)
            {
                BasketItem newItem = new BasketItem
                {
                    BasketID = customer.BasketID,
                    StockID = itemID,
                    Quantity = 1
                };
                _database.BasketItems.Add(newItem);
                await _database.SaveChangesAsync();
            }
            else
            {
                item.Quantity += 1;
                _database.Attach(item).State = EntityState.Modified;
                try
                {
                    await _database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw new Exception($"Basket not found!", e);
                }
            }
            return RedirectToPage();
        }
    }
}
