using AutoMapper;
using Manager.MessageBus;
using Manager.Services.OrderAPI.DbContexts;
using Manager.Services.OrderAPI.Extension;
using Manager.Services.OrderAPI.Messaging;
using Manager.Services.OrderAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


//52.1 - Asegurando el API con Authentication.
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:44349/";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };



    });



//52.2 - Asegurando el API con Authorizacion.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "HNL");
    });
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Manager.Services.OrderAPI", Version = "v1" });

    //52.3 - Para habilitar la autorizacion en el swagger
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] and your token ",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    //52.4 Se agrega la Seguridad Requerida. 
    //Es la manera en que se habilita la seguridad en el swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme="oauth2",
                Name="Bearer",
                In=ParameterLocation.Header
            },
            new List<string>()
        }
    });

});



//Conexion a la DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));



//Habilitando el AutoMapper
//IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
//builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



//Habilitamos el servicio de producto
builder.Services.AddScoped<IOrderRepository, OrderRepository>();



//Se agrega el option builder y permite eliminar el AddScoped de Order reporitory para 
//poder trabajar con el service Bus en OrderRepository
//video 126
var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseSqlServer(connectionString);
builder.Services.AddSingleton(new OrderRepository(optionBuilder.Options));

//Vid131 .2 Instanciamos los metodos del azure service bus
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

//Vid143. 2 Instanciado IMessageBus para integrar PaymentStatus
builder.Services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

//59.2 Habilitamos en el api la lectura del token
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//Vid131 .3 Realizamos el llamado a los metodos del service bus
app.UseAzureServiceBusConsumer();

app.Run();
