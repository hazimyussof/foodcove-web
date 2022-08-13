using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CO5227_J790290_1919281.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Stripe;

namespace CO5227_J790290_1919281.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private readonly AppDbContext _database;
        private readonly UserManager<ApplicationUser> _UserManager;
        public IList<CheckoutItems> Items { get; private set; }
        public OrderHistory Order = new OrderHistory();

        public decimal Total = 0;
        public long AmountPayable = 0;

        public CheckoutModel(AppDbContext database, UserManager<ApplicationUser> UserManager)
        {
            _database = database;
            _UserManager = UserManager;
        }

        public async Task OnGetAsync()
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _database
            .CheckoutCustomers
            .FindAsync(user.Email);

            Items = _database.CheckoutItems.FromSqlRaw(
                "SELECT Meals.ID, Meals.Price, " + "Meals.Name, " +
                "BasketItems.BasketID, BasketItems.Quantity " +
                "FROM Meals INNER JOIN BasketItems " +
                "ON Meals.ID = BasketItems.StockID " +
                "WHERE BasketID = {0}", customer.BasketID).ToList();

            Total = 0;
            foreach (var item in Items)
            {
                Total += (item.Quantity * item.Price);
            }
            AmountPayable = (long)(Total * 100);

        }

        public async Task Process()
        {
            var currentOrder = _database.OrderHistories.FromSqlRaw("SELECT * From OrderHistories").OrderByDescending(b => b.OrderNo).FirstOrDefault();

            if (currentOrder == null)
            {
                Order.OrderNo = 1;
            }
            else
            {
                Order.OrderNo = currentOrder.OrderNo + 1;
            }

            var user = await _UserManager.GetUserAsync(User);
            Order.Email = user.Email;
            _database.OrderHistories.Add(Order);

            CheckoutCustomer customer = await _database.CheckoutCustomers.FindAsync(user.Email);

            var basketItems = _database.BasketItems.FromSqlRaw("SELECT * From BasketItems WHERE BasketID = {0}", customer.BasketID).ToList();

            foreach (var item in basketItems)
            {
                Data.OrderItem oi = new Data.OrderItem
                {
                    OrderNo = Order.OrderNo,
                    StockID = item.StockID,
                    Quantity = item.Quantity
                };
                _database.OrderItems.Add(oi);
                _database.BasketItems.Remove(item);
            }
            await _database.SaveChangesAsync();
        }

        // Increase increment button method:
        public async Task<IActionResult> OnPostPlusAsync(int itemID)
        {
            var user = await _UserManager.GetUserAsync(User);CheckoutCustomer customer = await _database.CheckoutCustomers.FindAsync(user.Email);
            var item = _database.BasketItems.FromSqlRaw("SELECT * FROM BasketItems WHERE StockID = {0} AND BasketID = {1}", itemID, customer.BasketID).ToList().FirstOrDefault();

            item.Quantity += 1;
            await _database.SaveChangesAsync();
            return RedirectToPage();
        }

        // Decrease increment button method:
        public async Task<IActionResult> OnPostMinusAsync(int itemID)
        {
            var user = await _UserManager.GetUserAsync(User); CheckoutCustomer customer = await _database.CheckoutCustomers.FindAsync(user.Email);
            var item = _database.BasketItems.FromSqlRaw("SELECT * FROM BasketItems WHERE StockID = {0} AND BasketID = {1}", itemID, customer.BasketID).ToList().FirstOrDefault();

            if (item.Quantity == 1)
            {
                _database.BasketItems.Remove(item);
            }
            else
            {
                item.Quantity -= 1;
            }
            await _database.SaveChangesAsync();
            return RedirectToPage();
        }

        public IActionResult OnPostCharge(string stripeEmail,string stripeToken,long amount)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = amount,
                Description = "CO5227 Foodcove Charge",
                Currency = "gbp",
                Customer = customer.Id
            });
            Process().Wait();
            return RedirectToPage("/PurchaseConfirmation");
        }



    }
}
