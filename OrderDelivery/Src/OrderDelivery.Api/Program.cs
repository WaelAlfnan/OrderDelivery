using Microsoft.OpenApi.Models;
using OrderDelivery.Api.Filters;
using OrderDelivery.Api.Middleware;
using OrderDelivery.Application.Extensions;
using OrderDelivery.Infrastructure.Extensions;
using System.Reflection;

namespace OrderDelivery.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                // Add global filters
                // options.Filters.Add<ModelStateValidationFilter>(); // Temporarily removed for JWT testing
                options.Filters.Add<GlobalExceptionFilter>();
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Enhanced Swagger configuration
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "OrderDelivery API",
                    Version = "v1",
                    Description = "API for Order Delivery System"
                });

                // Add XML comments if available
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Configure Swagger to handle nullable reference types
                c.SupportNonNullableReferenceTypes();

                // Add security definitions if needed
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Add Infrastructure services FIRST (Database, Identity, etc.)
            try
            {
                builder.Services.AddInfrastructureServices(builder.Configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding infrastructure services: {ex.Message}");
                throw;
            }

            // Add Application services SECOND (Auth, JWT, SMS, Validators, etc.)
            try
            {
                builder.Services.AddApplicationServices(builder.Configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding application services: {ex.Message}");
                throw;
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            try
            {
                // Enable Swagger in all environments (Development, Staging, Production)
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderDelivery API v1");
                    options.RoutePrefix = "swagger";
                    options.DocumentTitle = "OrderDelivery API Documentation";
                    
                    // Additional security for production
                    if (!app.Environment.IsDevelopment())
                    {
                        // Hide the "Try it out" button in production for security
                        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                        options.DefaultModelsExpandDepth(-1);
                    }
                });

                Console.WriteLine($"Swagger configured successfully for {app.Environment.EnvironmentName} environment");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring Swagger: {ex.Message}");
                Console.WriteLine($"Swagger stack trace: {ex.StackTrace}");
            }

            // HTTPS redirection for all environments except development
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            // Add CORS if needed
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // Authentication & Authorization middleware MUST come before exception handling
            Console.WriteLine("Configuring Authentication middleware...");
            app.UseAuthentication();
            Console.WriteLine("Configuring Authorization middleware...");
            app.UseAuthorization();
            Console.WriteLine("Authentication and Authorization middleware configured successfully");

            // إضافة Middleware لمعالجة الاستثناءات بشكل مركزي
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.MapControllers();

            // Seed default roles
            try
            {
                await app.SeedRolesAsync();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error seeding roles. This might be expected if the database is not accessible.");
            }

            app.Run();
        }
    }
}
