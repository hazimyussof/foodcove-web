using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using CO5227_J790290_1919281.Data;
using Microsoft.EntityFrameworkCore;

namespace CO5227_J790290_1919281.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegistrationModel Input { get; set; }

        public CheckoutCustomer Customer = new CheckoutCustomer();
        public Basket Basket = new Basket();

        private AppDbContext _database;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;


        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, AppDbContext database)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _database = database;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    await _userManager.AddToRoleAsync(user, "Member");

                    NewBasket();
                    NewCustomer(Input.Email);
                    await _database.SaveChangesAsync();

                    return RedirectToPage("/Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public void NewBasket()
        {
            var currentBasket = _database.Baskets.FromSqlRaw("SELECT * From Baskets")
                .OrderByDescending(b => b.BasketID)
                .FirstOrDefault();
            if (currentBasket == null)
            {
                Basket.BasketID = 1;
            }
            else
            {
                Basket.BasketID = currentBasket.BasketID + 1;
            }

            _database.Baskets.Add(Basket);
        }

        public void NewCustomer(string Email)
        {
            Customer.Email = Email;
            Customer.BasketID = Basket.BasketID;
            _database.CheckoutCustomers.Add(Customer);
        }

        public void OnGet()
        {
        }
    }
}
