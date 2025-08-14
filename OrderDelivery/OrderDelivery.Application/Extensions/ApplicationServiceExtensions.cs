using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OrderDelivery.Application.Configurations;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Services;
using OrderDelivery.Application.Validators;
using OrderDelivery.Application.Validators.RegistrationSteps;
using Amazon.S3;
using Amazon.Runtime;
using Amazon;
using System.Text;

namespace OrderDelivery.Application.Extensions
{
    /// <summary>
    /// Extension methods for registering application services
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Registers all application services in the DI container
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register configuration classes
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<TwilioSettings>(configuration.GetSection("TwilioSettings"));
            services.Configure<S3Settings>(configuration.GetSection("S3"));

            // Register application services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ISmsService, TwilioSmsService>();
            services.AddScoped<IUserCreationService, UserCreationService>();
            services.AddScoped<IRegistrationStepsService, RegistrationStepsService>();

            // AWS S3 Client - Proper configuration with region and credentials
            var s3Settings = configuration.GetSection("S3").Get<S3Settings>();
            if (s3Settings != null && !string.IsNullOrEmpty(s3Settings.AccessKey) && !string.IsNullOrEmpty(s3Settings.SecretKey))
            {
                try
                {
                    // Validate S3 settings
                    if (string.IsNullOrEmpty(s3Settings.Region))
                    {
                        throw new InvalidOperationException("S3 region is not configured in appsettings.json");
                    }

                    if (string.IsNullOrEmpty(s3Settings.BucketName))
                    {
                        throw new InvalidOperationException("S3 bucket name is not configured in appsettings.json");
                    }

                    // Create AWS credentials
                    var credentials = new BasicAWSCredentials(s3Settings.AccessKey, s3Settings.SecretKey);
                    
                    // Create S3 client configuration with region
                    var s3Config = new AmazonS3Config
                    {
                        RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
                    };
                    
                    // Register the S3 client as a singleton with proper configuration
                    services.AddSingleton<IAmazonS3>(provider => new AmazonS3Client(credentials, s3Config));
                    
                    // File Storage Service
                    services.AddScoped<IFileStorageService, S3FileStorageService>();
                    
                    Console.WriteLine($"S3 service configured successfully for region: {s3Settings.Region}, bucket: {s3Settings.BucketName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configuring S3 service: {ex.Message}");
                    Console.WriteLine("S3 service will not be available. File uploads will fail.");
                    // Still register the service but it might fail at runtime
                    services.AddScoped<IFileStorageService, S3FileStorageService>();
                }
            }
            else
            {
                Console.WriteLine("Warning: S3 configuration is missing or incomplete. S3 service will not work properly.");
                Console.WriteLine("Please check your appsettings.json and environment variables.");
                // Still register the service but it might fail at runtime
                services.AddScoped<IFileStorageService, S3FileStorageService>();
            }

            // Register FluentValidation (auto-validation + assembly scanning)
            services.AddFluentValidationAutoValidation()
                    .AddValidatorsFromAssemblyContaining<LoginDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<ForgotPasswordDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<ResetPasswordDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<RegisterPhoneDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<VerifyPhoneDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<SetRoleDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<SetPasswordDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<MerchantInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<DriverInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<VehicleInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<ResidenceInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<UploadPhotoDtoValidator>();

            // Configure JWT Authentication
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings != null && !string.IsNullOrEmpty(jwtSettings.SecretKey))
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            }
            else
            {
                // Fallback authentication if JWT settings are not configured
                services.AddAuthentication();
            }

            return services;
        }
    }
}
