using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace OLP.Infrastructure.Seed
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration configuration)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            // Create roles if they don't exist
            string[] roles = { "Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole<Guid>(role));

                    if (!result.Succeeded)
                        throw new InvalidOperationException(
                            string.Join("; ",
                                result.Errors.Select(e => e.Description)));
                }
            }

            // Read initial Admin credentials from configuration
            var email = configuration["Admin:Email"];
            var password = configuration["Admin:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var existingAccount = await userManager.FindByEmailAsync(email);

            if (existingAccount != null)
            {
                if (!await userManager.IsInRoleAsync(existingAccount, "Admin"))
                    throw new InvalidOperationException(
                        "The configured Admin email belongs to a non-admin account.");

                return;
            }

            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, password);

            if (!createResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ",
                        createResult.Errors.Select(e => e.Description)));

            var roleResult = await userManager.AddToRoleAsync(admin, "Admin");

            if (!roleResult.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ",
                        roleResult.Errors.Select(e => e.Description)));

            // Your project has a separate Domain User profile
            var profileExists = await context.DomainUsers
                .AnyAsync(u => u.ApplicationUserId == admin.Id);

            if (!profileExists)
            {
                context.DomainUsers.Add(new User
                {
                    ApplicationUserId = admin.Id,
                    FirstName = "System",
                    LastName = "Admin"
                });

                await context.SaveChangesAsync();
            }
        }
    }
}

