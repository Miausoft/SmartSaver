using Microsoft.Extensions.DependencyInjection;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.Domain.Services.PasswordHash;
using SmartSaver.Domain.Services.Regex;
using SmartSaver.Domain.TokenValidation;
using SmartSaver.Domain.Repositories;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDomainLibrary(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IPasswordRegex, PasswordRegex>();
            services.AddScoped<ITokenValidationService, TokenValidationService>();
            services.AddScoped<IMailer, Mailer>();
            services.AddScoped<IRepository<User>, Repository<User>>();
            services.AddScoped<IRepository<Account>, Repository<Account>>();
            services.AddScoped<IRepository<Transaction>, Repository<Transaction>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();

            return services;
        }
    }
}
