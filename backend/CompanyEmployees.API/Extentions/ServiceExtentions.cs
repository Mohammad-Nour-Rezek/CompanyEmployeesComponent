using Contracts;
using Entities;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.API.Extentions
{
    public static class ServiceExtentions
    {
        // CORS
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                )

            // For production configuration
            //options.AddPolicy("CorsPolicy", builder =>
            //    builder.WithOrigins("https://localhost:3000")
            //           .WithMethods("GET", "POST")
            //           .WithHeaders("accept", "content-type")
            //)
            );
        }

        // IIS for Deployment [if not implemented default is self hosted on iis express]
        public static void ConfigureIISIntegration(this IServiceCollection services) => 
            services.Configure<IISOptions>(options => { }); // Return empty object means use default config

        public static void ConfigureLoggerService(this IServiceCollection services) => 
            services.AddScoped<ILoggerManager, LoggerManager>();

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) => 
            services.AddDbContext<RepositoryContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                                                b => b.MigrationsAssembly("CompanyEmployees.API")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) => 
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
            builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config => 
            { 
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.mnour.hateoas+json");
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.mnour.apiroot+json");
                }

                var xmlOutputFormatter = config.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault(); 
                
                if (xmlOutputFormatter != null) 
                { 
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.mnour.hateoas+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.mnour.apiroot+xml");
                }
            });
        }
    }
}
