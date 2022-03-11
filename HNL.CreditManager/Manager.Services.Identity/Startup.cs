using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Services;
using Manager.Services.Identity.DbContexts;
using Manager.Services.Identity.Initializer;
using Manager.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Manager.Services.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            }).AddInMemoryIdentityResources(SD.IdentityResources)
            .AddInMemoryApiScopes(SD.ApiScopes)
            .AddInMemoryClients(SD.Clients)
            .AddAspNetIdentity<ApplicationUser>();

            services.AddScoped<IDbInitializer, DbInitializer>();
            //services.AddScoped<IProfileService, ProfileService>();
            builder.AddDeveloperSigningCredential();

            services.AddControllersWithViews();

        }

        public void Configure(IApplicationBuilder ApplicationDbContext, IWebHostEnvironment env) 
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                ApplicationDbContext.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                ApplicationDbContext.UseHsts();
            }

            ApplicationDbContext.UseHttpsRedirection();
            ApplicationDbContext.UseStaticFiles();

            ApplicationDbContext.UseRouting();

            //50.3 Habilitando el identity
            ApplicationDbContext.UseIdentityServer();

            ApplicationDbContext.UseAuthorization();

            //Para implementar metodo seed




            //DbInitializer dbInitializer = new DbInitializer(,  );

            ApplicationDbContext.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        } 

    }
}
