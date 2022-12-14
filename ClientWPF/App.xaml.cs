using BusinnesLogicLayer.Infrastructure;
using BusinnesLogicLayer.Interfaces;
using BusinnesLogicLayer.ModelsDTO;
using BusinnesLogicLayer.Services;
using ClientWPF.ViewModels;
using ClientWPF.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider serviceProvider;
        private readonly string connectionString;
        public App()
        {
            var service = new ServiceCollection();
            connectionString = ConfigurationManager.ConnectionStrings["ContactDB"].ConnectionString;
            ConfigureServices(service);
            serviceProvider = service.BuildServiceProvider();

        }
        private void ConfigureServices(ServiceCollection collection)
        {
            collection.AddSingleton(typeof(IService<ContactDTO>), typeof(ContactService));
            collection.AddSingleton(typeof(ContactViewModel));
            collection.AddSingleton(typeof(MainWindow));
            ConfigurationBLL.ConfigureServices(collection, connectionString);
            ConfigurationBLL.AddDependecy(collection);
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = (Window)serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
