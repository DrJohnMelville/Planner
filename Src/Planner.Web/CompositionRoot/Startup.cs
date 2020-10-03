using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Repository.SqLite;
using TokenServiceClient.Website;

namespace Planner.Web.CompositionRoot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureDataProtection(services);
            AddCapWebAuthentication(services);
            DatabaseFactory.ConfigureDatabase(services);
            services.AddScoped<IDatedRemoteRepository<PlannerTask>, SqlRemoteRepositoryWithDate<PlannerTask>>();
            services.AddScoped<IDatedRemoteRepository<Note>, SqlRemoteRepositoryWithDate<Note>>();

            services.AddControllersWithViews().AddJsonOptions(ConfigureJsonSerialization);
        }

        private static void ConfigureDataProtection(IServiceCollection services) =>
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(".\\App_Data"))
                .SetApplicationName("Hints")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(30));

        private void AddCapWebAuthentication(IServiceCollection services) =>
            services.AddCapWebTokenService(
                Configuration.GetValue<string>("TokenService:Name"),
                Configuration.GetValue<string>("TokenService:Secret"));

        private void ConfigureJsonSerialization(JsonOptions o)
        {
            o.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            o.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
        }

        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.AddCapWebAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
            });
        }
    }
}