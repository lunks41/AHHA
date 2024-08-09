using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Admin;
using AHHA.Application.IServices.Masters;
using AHHA.Infra.Data;
using AHHA.Infra.Repository;
using AHHA.Infra.Services;
using AHHA.Infra.Services.Admin;
using AHHA.Infra.Services.Masters;
using Microsoft.AspNetCore.Http;
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
        serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());

        #region Services

        serviceCollection.AddScoped<IBaseService, BaseService>();
        serviceCollection.AddScoped<IAuthService, AuthService>();

        #region Master Services
        serviceCollection.AddScoped<IProductService, ProductService>();
        serviceCollection.AddScoped<ICountryService, CountryService>();
        serviceCollection.AddScoped<IPortRegionService, PortRegionService>();
        
        #endregion

        #region Admin Services
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IModuleService, ModuleService>();
        serviceCollection.AddScoped<ICompanyService, CompanyService>();
        serviceCollection.AddScoped<ITransactionService, TransactionService>();
        #endregion

        #endregion

        //serviceCollection.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        //{
        //    var connectionStringPlaceHolder = builder.Configuration.GetConnectionString("DbConnection");
        //    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        //    var dbName = httpContextAccessor.HttpContext.Request.Headers["companyName"].First();
        //    var connectionString = connectionStringPlaceHolder.Replace("{dbName}", dbName);
        //    options.UseSqlServer(connectionString);
        //});

        serviceCollection.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var getconnectionstrigName = httpContextAccessor.HttpContext.Request.Headers["RegId"].First();

            var connectionString = configuration.GetConnectionString("DbConnection");
            options.UseSqlServer(connectionString,
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        //serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
            
        //    configuration.GetConnectionString("DbConnection"),
        //        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));//Why use this line (b=>b.MigrationsAssembly)?

        serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return serviceCollection;
    }
}