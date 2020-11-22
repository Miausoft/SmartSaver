using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace SmartSaver.MVC.ServiceCollectionExtensions
{
    public static class AuthenticationWithExternal
    {
        public static IServiceCollection AddAuthenticationWithExternal(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.IsEssential = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.LoginPath = "/Authentication/Login";
                    options.Cookie.Name = "UserCookie";
                    options.AccessDeniedPath = "/Home/Index";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
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