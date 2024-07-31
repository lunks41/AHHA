using AHHA.Application.CommonServices;
using AHHA.Application.IServices.Masters;
using AHHA.Infra.Data;
using AHHA.Infra.Repository;
using AHHA.Infra.Services.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AHHA.Infra.Extensions;

public static class InfraServices
{
    public static IServiceCollection AddInfraServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());

        #region Services

        serviceCollection.AddScoped<IProductService, ProductService>();
        serviceCollection.AddScoped<ICountryService, CountryService>();
        serviceCollection.AddScoped<IErrorLogServices, ErrorLogServices>();
        serviceCollection.AddScoped<IAuditLogServices, AuditLogServices>();

        #endregion

        serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
            configuration.GetConnectionString("DbConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));//Why use this line (b=>b.MigrationsAssembly)?

        serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        serviceCollection.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
       

        return serviceCollection;
    }
}