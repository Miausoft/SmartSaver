using Microsoft.Extensions.DependencyInjection;
using SmartSaver.Domain.Managers;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.Services.PasswordHash;
using SmartSaver.Domain.Services.Regex;
using SmartSaver.EntityFrameworkCore.Repositories;

namespace SmartSaver.Domain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDomainLibrary(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IPasswordRegex, PasswordRegex>();
            services.AddScoped<TransactionManager>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
            services.AddScoped<ITokenValidation, TokenValidation>();

            return services;
        }
    }
}
