using OrderDelivery.Api.Middleware;
using OrderDelivery.Application.Extensions;
using OrderDelivery.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace OrderDelivery.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Load environment variables from .env file if it exists
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
            if (File.Exists(envPath))
            {
                Console.WriteLine("Loading environment variables from .env file...");
                foreach (var line in File.ReadAllLines(envPath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;
                    
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        Environment.SetEnvironmentVariable(key, value);
                        Console.WriteLine($"Loaded environment variable: {key}");
                    }
                }
                Console.WriteLine("Environment variables loaded successfully.");
            }
            else
            {
                Console.WriteLine("No .env file found. Using system environment variables.");
            }

            // Log current AWS configuration for debugging
            var awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            
            if (!string.IsNullOrEmpty(awsAccessKey))
            {
                Console.WriteLine("AWS_ACCESS_KEY_ID is configured.");
            }
            else
            {
                Console.WriteLine("Warning: AWS_ACCESS_KEY_ID is not configured.");
            }
            
            if (!string.IsNullOrEmpty(awsSecretKey))
            {
                Console.WriteLine("AWS_SECRET_ACCESS_KEY is configured.");
            }
            else
            {
                Console.WriteLine("Warning: AWS_SECRET_ACCESS_KEY is not configured.");
            }

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

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
            if (app.Environment.IsDevelopment())
            {
                try
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderDelivery API v1");
                        options.RoutePrefix = "swagger";
                        options.DocumentTitle = "OrderDelivery API Documentation";
                    });
                    
                    Console.WriteLine("Swagger configured successfully for development environment");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configuring Swagger: {ex.Message}");
                    Console.WriteLine($"Swagger stack trace: {ex.StackTrace}");
                }
            }
            else
            {
                app.UseHttpsRedirection();
            }

            // Add CORS if needed
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // إضافة Middleware لمعالجة الاستثناءات بشكل مركزي
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Authentication & Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

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
