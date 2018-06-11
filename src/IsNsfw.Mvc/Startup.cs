using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using IsNsfw.Migration;
using IsNsfw.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Redis;

namespace IsNsfw.Mvc
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseServiceStack(new AppHost
            {
                AppSettings = new MultiAppSettings(
                                    File.Exists("~/appsettings.env".MapHostAbsolutePath()) ? new TextFileSettings("~/appsettings.env".MapHostAbsolutePath()) : new DictionarySettings(),
                                    new NetCoreAppSettings(Configuration),
                                    new AppSettings()
                                )
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "Link",
                    template: "{*id}",
                    defaults: new { controller = "Link", action = "Index" }
                    , constraints: new { id = new LinkKeyRouteConstraint() }
                );
            });

            var svcProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString(HostContext.AppSettings.GetString("ConnectionString"))
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(_20180601_InitializeDb).Assembly).For.Migrations())
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);

            var runner = svcProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations
            //runner.Rollback(1000);
            runner.MigrateUp();
        }
    }
}
