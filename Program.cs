using BooksSpring26.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BooksSpring26
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //1. Get the connection string from appsettings.json
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            //2. Register the DbContext with the dependency injection container 
            //define options such that our application is configured to use the DbContext class with SqlServer
            //and the connection string we retrieved from appsettings.json
            builder.Services.AddDbContext<BooksDbContext>(options => options.UseSqlServer(connString));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<BooksDbContext>().AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login"; // Set the login path for unauthenticated users
                options.LogoutPath = $"/Identity/Account/Logout"; // Set the access denied path for unauthorized us\ers
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied"; // Set the access denied path for unauthorized users
            });

            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IEmailSender, EmailSender>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapStaticAssets();

            app.MapRazorPages(); //to map the razor pages for identity
            app.MapControllerRoute(
                name: "default",
                pattern: "{Area=Customer}/{controller=Home}/{action=Index}/{id?}") //default controller and action set to Home and Index, it may have id but it's optional
                .WithStaticAssets();

            app.Run();
        }
    }
}
