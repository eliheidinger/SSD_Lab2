using Lab1.Data;
using Lab1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lab1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Bind secrets from secrets.json
            var appSecrets = new AppSecrets();
            builder.Configuration.GetSection("Secrets").Bind(appSecrets);

            // Add services to the container with the secret connection string
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(appSecrets.DefaultConnection));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Store secrets in DbInitializer if needed
            //DbInitializer.appSecrets = appSecrets;

            // Seed roles and users
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                SeedRolesAndUsersAsync(userManager, roleManager, appSecrets).Wait();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }

        // Method to seed roles and users asynchronously
        private static async Task SeedRolesAndUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppSecrets secrets)
        {
            var roles = new[] { "Manager", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create Manager user
            var managerUser = new ApplicationUser
            {
                UserName = "manager@example.com",
                Email = "manager@example.com",
                FirstName = "ManagerFirstName",
                LastName = "ManagerLastName",
                PhoneNumber = "1234567890",
                EmailConfirmed = true
            };
            if (userManager.Users.All(u => u.UserName != managerUser.UserName))
            {
                var result = await userManager.CreateAsync(managerUser, secrets.ManagerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }

            // Create Employee user
            var employeeUser = new ApplicationUser
            {
                UserName = "employee@example.com",
                Email = "employee@example.com",
                FirstName = "EmployeeFirstName",
                LastName = "EmployeeLastName",
                PhoneNumber = "0987654321",
                EmailConfirmed = true
            };
            if (userManager.Users.All(u => u.UserName != employeeUser.UserName))
            {
                var result = await userManager.CreateAsync(employeeUser, secrets.EmployeePassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employeeUser, "Employee");
                }
            }
        }
    }
}