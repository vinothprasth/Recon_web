using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.ServiceModel;
using System;
using System.Reflection;
using System.IO;
using log4net;
using log4net.Config;
using Recon.Controllers;

namespace ReconWeb
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
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            services.AddControllersWithViews();
            services.AddMvc(option => option.EnableEndpointRouting = false);              
            services.AddSession();
            services.AddHttpContextAccessor();           
            services.AddRouting();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddLogging();
            services.AddTransient<CommonController>();
            services.AddHttpClient();

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
            app.UseSession();
            app.UseRouting();        
            app.UseAuthorization();
            app.UseMvc(routes =>
            {
               routes.MapRoute(
               name: "default",
               template: "{controller=Login}/{action=Login}/{id?}");
            });

        }
    }
}
