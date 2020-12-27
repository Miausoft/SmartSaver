using Microsoft.Extensions.DependencyInjection;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.Domain.Services.PasswordHash;
using SmartSaver.Domain.Services.Regex;
using SmartSaver.Domain.TokenValidation;
using SmartSaver.Domain.Repositories;

namespace SmartSaver.Domain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDomainLibrary(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IPasswordRegex, PasswordRegex>();
            services.AddScoped<IMailer, Mailer>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
            services.AddScoped<ITokenValidationService, TokenValidationService>();
            services.AddScoped<IAccountRepoository, AccountRepository>();
            services.AddScoped<ITransactionRepoositry, TransactionRepository>();

            return services;
        }
    }
}
