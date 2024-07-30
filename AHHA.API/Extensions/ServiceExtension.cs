

using AutoMapper;
using AHHA.API.ExceptionHandling;
using AHHA.API.Mapping;
using AHHA.Application.CommonServices;
using AHHA.Application.Extensions;
using AHHA.Application.Services.Masters.Countries;
using AHHA.Application.Services.Masters.Products;
using AHHA.Core.Entities.Masters;
using AHHA.Infra.Data;
using AHHA.Infra.Extensions;
using AHHA.Infra.Repository;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace AHHA.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IExceptionHandler, GlobalExcaptionHandler>();
            services.AddApplicationServices();//From Application Layar
            services.AddInfraServices(configuration); //From Infra Layar
            services.AddHealthChecks().Services.AddDbContext<ApplicationDbContext>();
            services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
