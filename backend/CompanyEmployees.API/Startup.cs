using CompanyEmployees.API.ActionFilters;
using CompanyEmployees.API.Extentions;
using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();

            services.ConfigureIISIntegration();

            services.ConfigureLoggerService();

            services.ConfigureSqlContext(Configuration);

            services.ConfigureRepositoryManager();

            services.AddAutoMapper(typeof(Startup));

            // services.AddMvc(); =-=> for mvc project
            // used to serve controllers cuz there is no views
            services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true; // to accept http Accept header
                config.ReturnHttpNotAcceptable = true; // return 406 not accepted if media typa in accept not supported
            }).AddNewtonsoftJson() // for use newton packages to use json patch doc
            .AddXmlDataContractSerializerFormatters() // to return serialized xml
            .AddCustomCSVFormatter();

            services.Configure<ApiBehaviorOptions>(option =>
                {
                    option.SuppressModelStateInvalidFilter = true; // to return 422 (Unprocessed Entity) insted of 400 (Bad Rquest) on invalid model state
                }
            );

            services.AddScoped<ValidationFilterAttribute>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // will add middleware for using HSTS, which adds the Strict-Transport-Security header.
                app.UseHsts();
            }

            app.ConfigureExceptionHandler(logger);

            app.UseHttpsRedirection();

            // enables using static files for the request. If we don’t set a path to the static files directory, it will use a wwwroot folder in our project by default.
            app.UseStaticFiles();

            // allow cross domain requests
            app.UseCors("CorsPolicy");

            // will forward proxy headers to the current request. This will help us during application deployment.
            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });

            // UseRouting, UseAuthorization: add routing and authorization features to our application, respectively.
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // adds an endpoint for the controller’s action to the routing without specifying any routes.
                endpoints.MapControllers();
            });
        }
    }
}
