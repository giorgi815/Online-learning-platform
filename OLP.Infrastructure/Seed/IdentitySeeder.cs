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

            var email = configuration["Admin:Email"];
            var password = configuration["Admin:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var admin = await userManager.FindByEmailAsync(email);

            if (admin is not null)
            {
                if (!await userManager.IsInRoleAsync(admin, "Admin"))
                    throw new InvalidOperationException(
                        "The configured Admin email belongs to a non-admin account.");
            }
            else
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(admin, password);
                if (!createResult.Succeeded)
                    throw new InvalidOperationException(string.Join("; ",
                        createResult.Errors.Select(e => e.Description)));

                var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
                if (!roleResult.Succeeded)
                    throw new InvalidOperationException(string.Join("; ",
                        roleResult.Errors.Select(e => e.Description)));
            }

            if (!await context.DomainUsers.AnyAsync(u => u.ApplicationUserId == admin.Id))
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

