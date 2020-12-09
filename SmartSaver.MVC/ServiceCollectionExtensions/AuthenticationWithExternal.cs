using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Builder;

namespace SmartSaver.MVC.ServiceCollectionExtensions
{
    public static class AuthenticationWithExternal
    {
        public static IServiceCollection AddAuthenticationWithExternal(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.ConsentCookie.IsEssential = true;
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.IsEssential = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.LoginPath = configuration["Cookie:LoginPath"];
                    options.Cookie.Name = configuration["Cookie:Name"];
                    options.AccessDeniedPath = configuration["Cookie:AccessDeniedPath"];
                    options.ExpireTimeSpan = TimeSpan.Parse(configuration["Cookie:MinutesToExpiration"]);
                })
                .AddGoogle(options =>
                {
                    options.ClientId = configuration["Google:ClientId"];
                    options.ClientSecret = configuration["Google:ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    options.AppId = configuration["Facebook:AppId"];
                    options.AppSecret = configuration["Facebook:AppSecret"];
                });

            return services;
        }
    }
}
