using Microsoft.Extensions.DependencyInjection;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.Domain.Services.PasswordHash;
using SmartSaver.Domain.Services.Regex;
using SmartSaver.Domain.Services.AuthenticationServices.Jwt;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.Services.Transactions;
using SmartSaver.Domain.Services.SavingSuggestions;
using SmartSaver.Domain.Services.SavingSuggestion;
using SmartSaver.EntityFrameworkCore.Models;

namespace SmartSaver.Domain
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDomainLibrary(this IServiceCollection services)
        {
            services.AddScoped<IAuthentication, Authentication>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IPasswordRegex, PasswordRegex>();
            services.AddScoped<ITokenAuthentication, TokenAuthentication>();
            services.AddScoped<ITransactionsService, TransactionsService>();
            services.AddScoped<IMailer, Mailer>();
            services.AddScoped<ISuggestions, Suggestions>();
            services.AddScoped<IRepository<User>, Repository<User>>();
            services.AddScoped<IRepository<Account>, Repository<Account>>();
            services.AddScoped<IRepository<Transaction>, Repository<Transaction>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();

            return services;
        }
    }
}
