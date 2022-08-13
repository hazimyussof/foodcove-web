using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CO5227_J790290_1919281.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Meal> Meals { get; set; }

        public DbSet<CheckoutCustomer> CheckoutCustomers { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }

        [NotMapped]
        public DbSet<CheckoutItems> CheckoutItems { get; set; }

        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<BasketItem>().HasKey(t => new { t.StockID, t.BasketID });
            builder.Entity<OrderItem>().HasKey(t => new { t.StockID, t.OrderNo });
        }
    }
}
