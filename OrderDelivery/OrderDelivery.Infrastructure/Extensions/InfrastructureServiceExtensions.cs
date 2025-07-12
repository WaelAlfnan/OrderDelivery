using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Infrastructure.Data.Contexts;

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
            // Register the DbContext with the connection string from configuration
            services.AddDbContext<OrderDeliveryDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("OrderDelivery.Infrastructure")));

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

            // Register repositories and unit of work
            //services.AddScoped<IGenericRepository, GenericRepository>();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
