using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tamagotchi.Competition.AppSettings;
using Tamagotchi.Competition.Context;
using Tamagotchi.Competition.Controllers;
using Tamagotchi.Competition.Providers.Event;
using Tamagotchi.Competition.Providers.Score;

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
            services.AddDbContext<TamagotchiCompetitionContext>(options =>
               options.UseNpgsql(Configuration.GetConnectionString("DB")));
            services.AddScoped<TamagotchiCompetitionContext>();
            services.AddScoped<IScoreProvider, ScoreProvider>();
            services.AddScoped<IEventProvider, EventProvider>();
            services.AddScoped<CompetitionController>();
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
               });
            IConfigurationSection appConfig = Configuration.GetSection(ConfigSections.APP_CONFIG);
            services.Configure<AppConfig>(appConfig);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            UpdateDatabase(app);
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
            //app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void UpdateDatabase(IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var ctx = scope.ServiceProvider.GetService<TamagotchiCompetitionContext>())
                {
                    ctx.Database.Migrate();
                }
            }
        }

    }
}
