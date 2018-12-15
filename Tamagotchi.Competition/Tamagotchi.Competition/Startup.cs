using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tamagotchi.Competition.Accessor;
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,

                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,

                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection(ConfigSections.SecretKey).Value))
               };
           });
            services.AddDbContext<TamagotchiCompetitionContext>(options =>
              options.UseNpgsql(Configuration.GetSection(ConfigSections.DATABASE).Value));
            services.AddScoped<TamagotchiCompetitionContext>();
            services.AddScoped<IScoreProvider, ScoreProvider>();
            services.AddScoped<IEventProvider, EventProvider>();
            services.AddScoped<CompetitionController>();
            IConfigurationSection appConfig = Configuration.GetSection(ConfigSections.APP_CONFIG);
            services.Configure<AppConfig>(appConfig);
            var corsBuilder = new CorsPolicyBuilder()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials();
            services.AddCors(options =>
            {
                options.AddPolicy(ConfigSections.CORS_POLICY, corsBuilder.Build());
            });
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
                           Email = "tamagotchi.vlsu@gmail.com"
                       },
                       TermsOfService = "http://swagger.io/terms/"
                   });
                   c.CustomSchemaIds(type => type.FriendlyId(true));
                   c.DescribeAllEnumsAsStrings();
               });         
            services.AddMvc(options =>
            {
                // All endpoints need authorization using our custom authorization filter
                options.Filters.Add(new AuthAccessor(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            UpdateDatabase(app);
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.RoutePrefix = "swagger/ui";
                   c.SwaggerEndpoint("/swagger/1.0.0/swagger.json", "Competition API");
               });
            loggerFactory.AddConsole(Configuration.GetSection(ConfigSections.LOGGING));
            loggerFactory.AddDebug();
            app.UseCors(ConfigSections.CORS_POLICY);
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
