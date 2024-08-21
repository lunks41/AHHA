using AHHA.Application.CommonServices;
using AHHA.Application.IServices;
using AHHA.Application.IServices.Admin;
using AHHA.Application.IServices.Masters;
using AHHA.Core.Common;
using AHHA.Infra.Data;
using AHHA.Infra.Repository;
using AHHA.Infra.Services;
using AHHA.Infra.Services.Admin;
using AHHA.Infra.Services.Masters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AHHA.Infra.Extensions;

public static class InfraServices
{
    public static IServiceCollection AddInfraServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        #region Services

        serviceCollection.AddScoped<IBaseService, BaseService>();
        serviceCollection.AddScoped<IAuthService, AuthService>();

        #region Master Services
        serviceCollection.AddScoped<IProductService, ProductService>();
        serviceCollection.AddScoped<ICountryService, CountryService>();
        serviceCollection.AddScoped<IPortRegionService, PortRegionService>();
        serviceCollection.AddScoped<ICOACategory1Service, COACategory1Service>();
        serviceCollection.AddScoped<ICOACategory2Service, COACategory2Service>();
        serviceCollection.AddScoped<ICOACategory3Service, COACategory3Service>();
        serviceCollection.AddScoped<IAccountSetupCategoryService, AccountSetupCategoryService>();
        serviceCollection.AddScoped<IAccountSetupService, AccountSetupService>();
        serviceCollection.AddScoped<IBankService, BankService>();
        serviceCollection.AddScoped<IBargeService, BargeService>();
        serviceCollection.AddScoped<ICategoryService, CategoryService>();
        serviceCollection.AddScoped<IChartOfAccountService, ChartOfAccountService>();
        serviceCollection.AddScoped<ICustomeGroupCreditLimtService, CustomeGroupCreditLimtService>();
        serviceCollection.AddScoped<ICustomerAddressService, CustomerAddressService>();
        serviceCollection.AddScoped<ICustomerContactService, CustomerContactService>();
        serviceCollection.AddScoped<ICustomerService, CustomerService>();
        serviceCollection.AddScoped<ICustomerCreditLimitService, CustomerCreditLimitService>();
        serviceCollection.AddScoped<IDepartmentService, DepartmentService>();
        serviceCollection.AddScoped<IDesignationService, DesignationService>();
        serviceCollection.AddScoped<IEmployeeService, EmployeeService>();
        serviceCollection.AddScoped<IGroupCreditLimit_CustomerService, GroupCreditLimit_CustomerService>();
        //serviceCollection.AddScoped<IGroupCreditLimitService, CustomeGroupCreditLimtService>();
        serviceCollection.AddScoped<IGstCategoryService, GstCategoryService>();
        //serviceCollection.AddScoped<IGstService, gst>();
        serviceCollection.AddScoped<IOrderTypeCategoryService, OrderTypeCategoryService>();
        serviceCollection.AddScoped<IOrderTypeService, OrderTypeService>();
        serviceCollection.AddScoped<IPaymentTypeService, PaymentTypeService>();
        serviceCollection.AddScoped<IPortService, PortService>();
        serviceCollection.AddScoped<ISubCategoryService, SubCategoryService>();
        serviceCollection.AddScoped<ISupplierAddressService, SupplierAddressService>();
        serviceCollection.AddScoped<ISupplierContactService, SupplierContactService>();
        serviceCollection.AddScoped<ISupplierService, SupplierService>();
        //serviceCollection.AddScoped<IUomService, uom>();
        serviceCollection.AddScoped<IVesselService, VesselService>();
        serviceCollection.AddScoped<IVoyageService, VoyageService>();

        #endregion Master Services

        #region Admin Services
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IModuleService, ModuleService>();
        serviceCollection.AddScoped<ICompanyService, CompanyService>();
        serviceCollection.AddScoped<ITransactionService, TransactionService>();
        #endregion Admin Services

        #region
        serviceCollection.AddScoped<ILookupService, LookupService>();
        #endregion Services

        #endregion

        serviceCollection.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            DBGetConnection dBGetConnection = new DBGetConnection();
            string regId = string.Empty;
            var connectionString = string.Empty;

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            regId = httpContextAccessor.HttpContext.Request.Headers["regId"].First().ToString();

            var getConnectionStringName = dBGetConnection.GetconnectionDB(regId);

            ////read the company registration data from json
            //string regCompanyData = File.ReadAllText("regCompany.json");
            ////Convert json to object list
            //var regCompany = JsonConvert.DeserializeObject<IEnumerable<CompanyRegistration>>(regCompanyData);
            //// find out the regId & get the connectionstring from there
            //var getConnectionStringName = regCompany.Where(b => b.RegId == regId).FirstOrDefault().ConnectionStringName;

            if (getConnectionStringName == null)
                connectionString = configuration.GetConnectionString(getConnectionStringName);
            else
                connectionString = configuration.GetConnectionString(getConnectionStringName);

            // connectionString = configuration.GetConnectionString("DbConnection");
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