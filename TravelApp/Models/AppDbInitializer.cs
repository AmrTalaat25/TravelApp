using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TravelApp.Models; // Make sure to include the appropriate namespace for your models

namespace TravelApp.Models
{
    public class AppDbInitializer
    {
        // Seeding roles
        public static async Task SeedRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                if (!await roleManager.RoleExistsAsync("CompanyOwner"))
                {
                    await roleManager.CreateAsync(new IdentityRole("CompanyOwner"));
                }
            }
        }

        //Seed Users
        public static async Task SeedUsersAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Create admin user, set password, assign roles
                if(await userManager.FindByNameAsync("admin") == null)
                {
                    var adminUser = new User
                    {
                        UserName = "admin",
                        FirstName = "Admin",
                        LastName = "User",
                        Country = "Country",
                        City = "City",
                        Address = "Address",
                        Email = "admin@gmail.com",
                    };
                    var password = "Admin123!";
                    await userManager.CreateAsync(adminUser, password);
                    await userManager.AddToRoleAsync(adminUser, "User");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Create company owner user
                if (await userManager.FindByNameAsync("company") == null)
                {
                    var companyUser = new User
                    {
                        UserName = "company",
                        FirstName = "Company",
                        LastName = "User",
                        Country = "Country",
                        City = "City",
                        Address = "Address",
                        Email = "company@gmail.com",
                    };
                    var password = "Company123!";
                    await userManager.CreateAsync(companyUser, password);
                    await userManager.AddToRoleAsync(companyUser, "User");
                    await userManager.AddToRoleAsync(companyUser, "CompanyOwner");
                }

                
                // Create regular user
                if (await userManager.FindByNameAsync("user1") == null)
                {
                    var user1 = new User
                    {
                        UserName = "user1",
                        FirstName = "Travel1",
                        LastName = "User1",
                        Country = "Country",
                        City = "City",
                        Address = "Address",
                        Email = "user1@gmail.com",
                    };
                    var password = "User123!";
                    await userManager.CreateAsync(user1, password);
                    await userManager.AddToRoleAsync(user1, "User");
                }
                if (await userManager.FindByNameAsync("user2") == null)
                {
                    var user2 = new User
                    {
                        UserName = "user2",
                        FirstName = "Travel2",
                        LastName = "User2",
                        Country = "Country",
                        City = "City",
                        Address = "Address",
                        Email = "user2@gmail.com",
                    };
                    var password = "User123!";
                    await userManager.CreateAsync(user2, password);
                    await userManager.AddToRoleAsync(user2, "User");
                }
            } 
        }

        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var existingCompanyUserIds = (await userManager.GetUsersInRoleAsync("CompanyOwner")).Select(u => u.Id).ToList();
                var existingUserIds = (await userManager.GetUsersInRoleAsync("User")).Select(u => u.Id).ToList();
                var existingAdminIds = (await userManager.GetUsersInRoleAsync("Admin")).Select(u => u.Id).ToList();

                // Company
                if (!context.Companies.Any() && existingCompanyUserIds.Any())
                {
                    context.Companies.AddRange(new Company()
                    {
                        UserID = existingCompanyUserIds.FirstOrDefault(),
                        CompanyName = "Company1",
                        CompanyAddress = "Address1",
                        ContactInformation = "Contact1",
                    });
                    context.SaveChanges();
                }
                var existingCompanyIds = context.Companies.Select(c => c.CompanyID).ToList();
                
                // Advertisement
                if (!context.Advertisements.Any())
                {
                    context.Advertisements.AddRange(new Advertisement()
                    {
                        CompanyID = existingCompanyIds.FirstOrDefault(),
                        CompanyName = "Company1",
                        Title = "Advertisement1",
                        price = 10.99m,
                        Description = "Description1",
                        ValidFrom = DateTime.Now.AddDays(-5),
                        ValidTo = DateTime.Now.AddDays(5),
                    },
                    new Advertisement()
                    {
                        CompanyID = existingCompanyIds.FirstOrDefault(),
                        CompanyName = "Company2",
                        Title = "Advertisement2",
                        price = 15.99m,
                        Description = "Description2",
                        ValidFrom = DateTime.Now.AddDays(-3),
                        ValidTo = DateTime.Now.AddDays(7),
                    });
                    context.SaveChanges();
                }
                var existingAdvertisementIds = context.Advertisements.Select(k => k.AdID).ToList();
                // Review
                if (!context.Reviews.Any())
                {
                    context.Reviews.AddRange(new Review()
                    {
                        AdID = existingAdvertisementIds.FirstOrDefault(),
                        UserID = existingUserIds.FirstOrDefault(),
                        Username = "user1",
                        Rating = 4,
                        Comment = "Good advertisement",
                        DatePosted = DateTime.Now.AddDays(-3),
                    },
                    new Review()
                    {
                        AdID = existingAdvertisementIds.Skip(1).FirstOrDefault(),
                        UserID = existingUserIds.Skip(1).FirstOrDefault(),
                        Username = "user2",
                        Rating = 5,
                        Comment = "Excellent advertisement",
                        DatePosted = DateTime.Now.AddDays(-2),
                    });
                    context.SaveChanges(); 
                }
            }
        }
    }
}
