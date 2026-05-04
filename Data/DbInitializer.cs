using Microsoft.AspNetCore.Identity;

namespace BooksSpring26.Data
{
    public static class DbInitializer
    {
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        private const string SeedAdminEmail = "admin@bookssite.local";
        private const string SeedAdminPassword = "Admin#12345";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in new[] { AdminRole, CustomerRole })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var admin = await userManager.FindByEmailAsync(SeedAdminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = SeedAdminEmail,
                    Email = SeedAdminEmail,
                    EmailConfirmed = true,
                    Name = "Site Admin"
                };
                var result = await userManager.CreateAsync(admin, SeedAdminPassword);
                if (!result.Succeeded)
                {
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
            {
                await userManager.AddToRoleAsync(admin, AdminRole);
            }
        }
    }
}
