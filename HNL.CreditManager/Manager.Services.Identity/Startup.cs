using Duende.IdentityServer.Services;
using Manager.Services.Identity.DbContexts;
using Manager.Services.Identity.Initializer;
using Manager.Services.Identity.Services;
using Manager.Services.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Manager.Services.Identity
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
            //50.1 Habilitamos el servicio del contexto 
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));



            //50.2 Habilitamos el servicio del ApplicationUser y IdentityRole
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            //50.3 Habilitamos el IdentityServer
            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.EmitStaticAudienceClaim = true;
                })
                .AddInMemoryIdentityResources(SD.IdentityResources)
                .AddInMemoryApiScopes(SD.ApiScopes)
                .AddInMemoryClients(SD.Clients)
                .AddAspNetIdentity<ApplicationUser>();

            //51.1 para inicializar la creacion de usuarios en el metodo seed.
            services.AddScoped<IDbInitializer, DbInitializer>();
            
            //59.1 Helper que permite agregar informacion al token de sus roles
            services.AddScoped<IProfileService, ProfileService>();

            //50.4 Para propositos de development agregamos el signinCredential
            builder.AddDeveloperSigningCredential();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
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

            //50.5 Habilitamos el IdentityServer
            app.UseIdentityServer();

            app.UseAuthorization();

            //51.3 Llama al metodo para 
            dbInitializer.Initialize();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
