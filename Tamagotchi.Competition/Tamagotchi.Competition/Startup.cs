using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using Tamagotchi.Competition.AppSettings;

namespace Tamagotchi.Competition
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_0);
            services
               .AddSwaggerGen(c =>
               {
                   c.SwaggerDoc("1.0.0", new Info
                   {
                       Version = "1.0.0",
                       Title = "Competition API",
                       Description = "Tamagotchi Competition API",
                       Contact = new Contact()
                       {
                           Name = "Swagger",
                           Url = "https://github.com/swagger-api/",
                           Email = "ppp@ppp.io"
                       },
                       TermsOfService = "http://swagger.io/terms/"
                   });
                   c.CustomSchemaIds(type => type.FriendlyId(true));
                   c.DescribeAllEnumsAsStrings();
                   //c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Environment.ApplicationName}.xml");                   
                   //c.DocumentFilter<BasePathFilter>("/v2");                  
               });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();
            app
               .UseMvc()
               .UseDefaultFiles()
               .UseStaticFiles()
               .UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.RoutePrefix = "swagger/ui";
                   c.SwaggerEndpoint("/swagger/1.0.0/swagger.json", "Competition API");
               });

            loggerFactory.AddConsole(Configuration.GetSection(ConfigSections.LOGGING));
            loggerFactory.AddDebug();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
