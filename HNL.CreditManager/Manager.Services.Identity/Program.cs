using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Services;
using Manager.Services.Identity;
using Manager.Services.Identity.DbContexts;
using Manager.Services.Identity.Initializer;
using Manager.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);



//1.1 SECCION PARA TRABAJAR CON LA CLASE "startup"
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

////Conexion a la DB
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

////Habilitamos el servicio del Identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

////50.2 Habilitamos el servicio del Identity, Configuraciones
//builder.Services.AddIdentityServer(options =>
//{
//    options.Events.RaiseErrorEvents = true;
//    options.Events.RaiseInformationEvents = true;
//    options.Events.RaiseFailureEvents = true;
//    options.Events.RaiseSuccessEvents = true;
//    options.EmitStaticAudienceClaim = true;
//}).AddInMemoryIdentityResources(SD.IdentityResources)
//            .AddInMemoryApiScopes(SD.ApiScopes)
//            .AddInMemoryClients(SD.Clients)
//            .AddAspNetIdentity<ApplicationUser>();


//builder.Services.AddScoped<IDbInitializer, DbInitializer>();
//builder.Services.AddScoped<IProfileService, ProfileService>();
//builder.AddDeveloperSigningCredential();





// Add services to the container.
//builder.Services.AddControllersWithViews();



var app = builder.Build();




//1.2 SECCION PARA TRABAJAR CON LA CLASE "startup"
startup.Configure(app, app.Environment);

//SECCION QUE SE PASA A LA CLASE STARTUP
//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

////50.3 Habilitando el identity
//app.UseIdentityServer();

//app.UseAuthorization();



//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
