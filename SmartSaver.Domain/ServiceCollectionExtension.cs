using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartSaver.Domain.Entities;
using SmartSaver.Domain.Services.AuthenticationServices;
using SmartSaver.Domain.Services.EmailServices;
using SmartSaver.Domain.Services.PasswordHash;
using SmartSaver.Domain.Services.Regex;

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

            return services;
        }
    }
}
