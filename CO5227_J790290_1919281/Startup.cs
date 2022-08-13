using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CO5227_J790290_1919281.Data;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace CO5227_J790290_1919281
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddDbContext<AppDbContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("AppDbContext")));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                options.LogoutPath = new PathString("/Index");
            });
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];

            CreateRoles(serviceProvider).Wait();

            // Use secure sockets
            app.UseHttpsRedirection();
            app.UseRouting();

            // Allows us to use CSS and Javascript files
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            // Tells project we're using RazorPages
            app.UseEndpoints(endpoints => { endpoints.MapRazorPages();  });
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.
                GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.
                GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Member" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database
                    var roleResult = await RoleManager.
                        CreateAsync(new IdentityRole(roleName));
                }
            }
            var _user = await UserManager.FindByEmailAsync("Admin@Chester.ac.uk");
            if (_user != null)
            {
                await UserManager.AddToRoleAsync(_user, "Admin");
            }
        }

    }
}
