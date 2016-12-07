using Identity.Data;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Identity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("dbsettings.json", optional: false, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
                    });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            // Add application services.
            services.AddSingleton<IEmailSender>(new AuthMessageSender(Configuration));
            services.AddSingleton<IGitService>(new GitService(@"git@acer"));
            //services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "Error",
                    template: "Error/{id?}",
                    defaults: new { controller = "Error",action = "ErrorParser"}
                );
                routes.MapRoute(
                    name: "Profile",
                    template: "{userName}",
                    defaults: new { controller = "Home", action = "Profile" }
                );
                routes.MapRoute(
                        name: "CommitInfo",
                        template:"{userName}/{repoName}/{branch}/{hash:minlength(40):maxlength(40)}",
                        defaults: new {controller = "Home", action = "CommitInfo"});
                routes.MapRoute(
                    name: "RepoInfo",
                    template: "Info/{userName}/{repoName}/{branch}/{*path}",
                    defaults: new { controller = "Home", action = "RepoInfo" });
                routes.MapRoute(
                    name: "ViewFile",
                    template: "View/{userName}/{repoName}/{branch}/{*path}",
                    defaults: new { controller = "Home", action = "ViewFile" });
                routes.MapRoute(
                    name: "RepoView",
                    template: "{userName}/{repoName}/{branch}/{*path}",
                    defaults: new { controller = "Home", action = "RepoView" });
            });
        }
    }
}