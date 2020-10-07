using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartSaver.Domain;
using SmartSaver.Domain.Entities;
using SmartSaver.EntityFrameworkCore;

namespace SmartSaver
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

#pragma warning disable EMB026 // UseStaticMethod
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDomainLibrary();
            services.AddDbContext<ApplicationDbContext>();
            services.AddSingleton<MainWindow>();
        }
#pragma warning restore EMB026 // UseStaticMethod

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
