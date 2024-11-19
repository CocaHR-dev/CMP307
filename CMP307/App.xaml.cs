using CMP307.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CMP307.TemplateSelectors;
using CMP307.ViewModels;
using CMP307.Services;

namespace CMP307
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public App()
        {
            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("Properties\\appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register configuration
            services.AddSingleton(_configuration);

            // Register DbContext with connection string from configuration
            services.AddDbContext<ScottishGlenContext>(options =>
                options.UseMySql(_configuration.GetConnectionString("ScottishGlenDb"), ServerVersion.AutoDetect(_configuration.GetConnectionString("ScottishGlenDb"))));

            // Register template selectors
            services.AddSingleton<AddEditButtonTemplateSelector>();
            services.AddSingleton<DeleteButtonTemplateSelector>();

            // Register services
            services.AddTransient<DepartmentService>();
            services.AddTransient<EmployeeService>();
            services.AddTransient<HardwareService>();

            // Register ViewModels
            services.AddTransient<EmployeeViewModel>();
            services.AddTransient<HardwareViewModel>();
            services.AddTransient<DepartmentViewModel>();

            // Register MainWindow
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Ensure database is created and migrated
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ScottishGlenContext>();
                dbContext.Database.Migrate();
            }

            // Show the main window
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Handle non-UI thread exceptions
            MessageBox.Show($"An unhandled exception occurred: {e.ExceptionObject}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // Handle UI thread exceptions
            MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
