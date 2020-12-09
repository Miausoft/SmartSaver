using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartSaver.EntityFrameworkCore;
using SmartSaver.MVC.ServiceCollectionExtensions;
using System.Collections.Generic;
using System.Globalization;
using SmartSaver.Domain;

namespace SmartSaver.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthenticationWithExternal(Configuration);

            services.AddRazorPages().AddMvcOptions(options =>
            {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "The field is required.");
            });

            services.AddMvc();
          
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AzureSqlServer")));
          
            services.AddDomainLibrary();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var locale = Configuration["Localization:Culture"];
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> { new CultureInfo(locale) },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo(locale) },
                DefaultRequestCulture = new RequestCulture(locale)
            };
            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(Configuration["HandlingPaths:Error"]);
                app.UseStatusCodePagesWithReExecute(Configuration["HandlingPaths:StatusCode"]);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}