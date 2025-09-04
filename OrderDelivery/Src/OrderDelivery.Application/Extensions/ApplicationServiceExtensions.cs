using Amazon;
using Amazon.S3;
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
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ISmsService, TwilioSmsService>();
            services.AddScoped<IUserCreationService, UserCreationService>();
            services.AddScoped<IRegistrationStepsService, RegistrationStepsService>();
            services.AddScoped<IFileStorageService, S3FileStorageService>();

            // AWS S3 Client - Proper configuration with region and credentials
            services.AddSingleton<IAmazonS3>(provider =>
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.EUCentral1, // Change to your preferred region
                    ForcePathStyle = false,
                    UseHttp = false
                };
                return new AmazonS3Client(config);
            });

            // Register FluentValidation
            services.AddFluentValidationAutoValidation()
                    .AddFluentValidationClientsideAdapters()
                    .AddValidatorsFromAssemblyContaining<ForgotPasswordDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<VerifyCodeDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<SetNewPasswordDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<ResendCodeDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<LoginDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<RefreshTokenRequestValidator>()
                    .AddValidatorsFromAssemblyContaining<UploadPhotoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<RegisterPhoneDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<VerifyPhoneDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<SetPasswordDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<SetRoleDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<SetBasicInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<MerchantInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<DriverInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<VehicleInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<ResidenceInfoDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<CompleteRegistrationDtoValidator>()
                    .AddValidatorsFromAssemblyContaining<PasswordResetSessionDtoValidator>();

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

                    // Add logging for debugging
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine($"JWT Token validated successfully for user: {context.Principal?.Identity?.Name}");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine($"JWT Challenge issued: {context.Error}, {context.ErrorDescription}");
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = context =>
                        {
                            Console.WriteLine($"JWT Message received: {context.Token}");
                            return Task.CompletedTask;
                        }
                    };

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
