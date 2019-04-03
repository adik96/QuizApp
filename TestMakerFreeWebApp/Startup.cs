using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestMakerFreeWebApp.Data;

namespace TestMakerFreeWebApp
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Dodanie obs³ugi Entity Framework zwi¹zanej z SqlServer.
            services.AddEntityFrameworkSqlServer();
            // Dodanie ApplicationDbContext.
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(
                new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    // Wy³¹czenie stosowania pamiêci podrêcznej dla wszystkich plików statycznych
                    context.Context.Response.Headers["Cache-Control"] =
                    Configuration["StaticFiles:Headers:Cache-Control"];
                    context.Context.Response.Headers["Pragma"] = 
                    Configuration["StaticFiles:Headers:Pragma"];
                    context.Context.Response.Headers["Expires"] = 
                    Configuration["StaticFiles:Headers:Expires"];
                }
            });
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
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

            // Utwórz zakres us³ugi, aby otrzymaæ instancjê ApplicationDbContext dziêki wstrzykiwaniu zale¿noœci
            using (var serviceScope =
            app.ApplicationServices.GetRequiredService<IServiceScopeFactory>
            ().CreateScope())
            {
                var dbContext =
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                // Utwórz bazê danych, jeœli nie istnieje, i zastosuj wszystkie oczekuj¹ce migracje
                dbContext.Database.Migrate();
                // Wype³nij bazê danymi pocz¹tkowymi
                DbSeeder.Seed(dbContext);
            }
        }
    }
}
