﻿using AHHA.API.Mapping;
using AHHA.Infra.Data;
using AHHA.Infra.Extensions;
using System.Reflection;

namespace AHHA.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfraServices(configuration); //From Infra Layar
            services.AddHealthChecks().Services.AddDbContext<ApplicationDbContext>();
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}