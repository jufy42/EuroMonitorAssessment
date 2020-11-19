using System;
using Autofac;
using Library.Core;
using Library.Injection;
using Library.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Library
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
            services
                .AddIdentity<SystemUser, SystemRole>()
                .AddUserStore<SystemUserStore>()
                .AddRoleStore<SystemRoleStore>()
                .AddUserValidator<SystemUserValidator<SystemUser>>()
                .AddPasswordValidator<SystemPasswordValidator<SystemUser>>()
                .AddSignInManager<SystemSignInManager<SystemUser>>();

            services.Configure<CookiePolicyOptions>(options =>
            {                                
                options.MinimumSameSitePolicy = SameSiteMode.None;                                
            });

            services.Configure<FormOptions>(x => x.ValueCountLimit = int.MaxValue);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = new PathString("/account/login");                        
                        options.AccessDeniedPath = new PathString("/account/denied");     
                        options.ExpireTimeSpan = TimeSpan.FromDays(10);
                        options.SlidingExpiration = true;
                        options.Cookie.Expiration = TimeSpan.FromDays(10);
                    });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/account/login");
                options.ExpireTimeSpan = TimeSpan.FromDays(10);
                options.SlidingExpiration = true;
            });

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddLogging(logging => logging                        
                .AddConsole()
                .AddDebug()                
                .AddFilter("System", LogLevel.Warning)
                .AddFilter<DebugLoggerProvider>("Microsoft", LogLevel.Warning)
                .AddConfiguration(Configuration.GetSection("Logging"))    
                .AddFile("Library.log")
            );
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            new DependencyInjection().BuildDependencies(builder);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
#if (!TEST)
                app.UseRewriter(new RewriteOptions().AddRedirectToHttps(StatusCodes.Status301MovedPermanently, 443));
#endif
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (ctx, next) =>
            {
                await next();

                if(ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {                    
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "home/error";
                    await next();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
