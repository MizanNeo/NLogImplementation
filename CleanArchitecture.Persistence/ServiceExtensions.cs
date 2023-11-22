using CleanArchitecture.Application.Repositories;
using CleanArchitecture.Persistence.Context;
using CleanArchitecture.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace CleanArchitecture.Persistence
{
    public static class ServiceExtensions
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //if (string.IsNullOrEmpty(connectionString))
            //{
            //    throw new ConfigurationErrorsException("DefaultConnection is missing or empty in appsettings.json");
            //}

            try
            {
                services.AddDbContext<DataContext>(opt => opt.UseSqlServer(connectionString));
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddScoped<IUserRepository, UserRepository>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error configuring persistence layer.", ex);
            }
        }
    }
}
