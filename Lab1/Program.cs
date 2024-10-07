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

            // Add services to the container with the connection string from appsettings.json
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            //trying to user key vault in older version of azure, couldnt work at all
            //var kvUri = new Uri(builder.Configuration.GetSection("KVURI").Value);

            //var azCred = new DefaultAzureCredential();

            //try
            //{
            //    builder.Configuration.AddAzureKeyVault(kvUri, azCred);
            //    ManagerPassword = builder.Configuration["ManagerPassword"];
            //    EmployeePassword = builder.Configuration["EmployeePassword"];
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error retrieving secrets from Key Vault: {ex.Message}");
            //}

            //ManagerPassword = "ManagerPassword1!";
            //EmployeePassword = "EmployeePassword1!";

            var app = builder.Build();

            // Bind configuration from appsettings.json
            var configuration = app.Services.GetService<IConfiguration>();
            var secrets = configuration.GetSection("Secrets").Get<AppSecrets>();

            // Seed roles and users
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // Pass secrets to DbInitializer
                DbInitializer.SeedUsersAndRoles(services, secrets).Wait();
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

       
    }
}