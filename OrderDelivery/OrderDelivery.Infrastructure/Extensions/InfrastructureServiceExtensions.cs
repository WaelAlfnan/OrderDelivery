using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderDelivery.Domain;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Domain.Interfaces;
using OrderDelivery.Infrastructure.Data;
using OrderDelivery.Infrastructure.Data.Contexts;
using OrderDelivery.Infrastructure.Data.Repositories;

namespace OrderDelivery.Infrastructure.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        /// <summary>
        /// Registers the OrderDelivery DbContext and related services with the dependency injection container
        /// </summary>
        /// <param name="services">The service collection to add services to</param>
        /// <param name="configuration">The configuration instance containing connection strings</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            try
            {
                // Database
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                }

                services.AddDbContext<OrderDeliveryDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Register Identity services
                services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    // User settings
                    options.User.AllowedUserNameCharacters = "0123456789+";
                    options.User.RequireUniqueEmail = true;

                    // SignIn settings
                    options.SignIn.RequireConfirmedAccount = true;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = true;
                })
                .AddEntityFrameworkStores<OrderDeliveryDbContext>()
                .AddDefaultTokenProviders();

                // Repositories
                services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                return services;
            }
            catch (Exception ex)
            {
                // Log the error and rethrow
                throw new InvalidOperationException("Failed to configure infrastructure services", ex);
            }
        }

        /// <summary>
        /// Seeds default roles in the database
        /// </summary>
        /// <param name="app">The web application instance</param>
        /// <returns>The web application for method chaining</returns>
        public static async Task<IApplicationBuilder> SeedRolesAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var roles = new[] { "Driver", "Merchant", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            return app;
        }
    }
}
