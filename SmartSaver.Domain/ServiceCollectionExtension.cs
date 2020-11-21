using Microsoft.Extensions.DependencyInjection;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.Domain.Services.PasswordHash;
using SmartSaver.Domain.Services.Regex;
using SmartSaver.Domain.Managers;

namespace SmartSaver.Domain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDomainLibrary(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IMailer, Mailer>();
            services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
            services.AddSingleton<IPasswordRegex, PasswordRegex>();
            services.AddScoped<TransactionManager>();

            return services;
        }
    }
}
