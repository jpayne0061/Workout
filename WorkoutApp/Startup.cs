using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkoutApp.Models;

namespace WorkoutApp
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

            services.AddDbContext<WorkoutContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("Workout")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Index",
                    template: "{controller=Workout}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "Create",
                    template: "{controller=Workout}/{action=Create}/{id?}");

                routes.MapRoute(
                    name: "History",
                    template: "{controller=Workout}/{action=ViewHistory}/{id?}");

                //routes.MapRoute(
                //    name: "Exercises",
                //    template: "{controller=Workout}/{action=Exercises}/{id?}");

                //routes.MapRoute(
                //    name: "Details",
                //    template: "{controller=Workout}/{action=Details}/{id?}");

                //routes.MapRoute(
                //    name: "Create",
                //    template: "{controller=Workout}/{action=Create}");

                //routes.MapRoute(
                //    name: "CreateExercise",
                //    template: "{controller=Workout}/{action=CreateExercise}/{id?}");

                //routes.MapRoute(
                //    name: "Start",
                //    template: "{controller=Workout}/{action=Start}/{id?}");

                //routes.MapRoute(
                //    name: "CompleteSet",
                //    template: "{controller=Workout}/{action=CompleteSet}/{id?}");
            });
        }
    }
}
