using DataLayer.DataContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinnesLogicLayer.Infrastructure
{
    public class ConfigurationBLL
    {
        public static void ConfigureServices(ServiceCollection services, string connection)
        {
            services.AddDbContext<ContactContext>(opt => opt.UseSqlServer(connection));
        }
        public static void AddDependecy(ServiceCollection services)
        {
            services.AddSingleton(typeof(DataLayer.Interfaces.IWorkContact), typeof(DataLayer.WorkTemp.WorkContact));
        }
    }
}
