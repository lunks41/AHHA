using System.Reflection;
using AHHA.Application.CommonServices;
using AHHA.Application.Services.Masters.Countries;
using AHHA.Application.Services.Masters.Products;
using Microsoft.Extensions.DependencyInjection;

namespace AHHA.Application.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());

        #region Services

        serviceCollection.AddScoped<IProductService, ProductService>();
        serviceCollection.AddScoped<ICountryService, CountryService>();
        serviceCollection.AddScoped<IErrorLogServices, ErrorLogServices>();
        serviceCollection.AddScoped<IAuditLogServices, AuditLogServices>();

        #endregion

        return serviceCollection;
    }
}