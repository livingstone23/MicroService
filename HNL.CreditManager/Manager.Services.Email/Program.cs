using Manager.Services.Email.DbContexts;
using Manager.Services.Email.Extension;
using Manager.Services.Email.Messaging;
using Manager.Services.Email.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



//Vid156 Conexion a la DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));




//Vid156 Habilitamos el servicio de producto
builder.Services.AddScoped<IEmailRepository, EmailRepository>();



//Se agrega el option builder y permite eliminar el AddScoped de Order reporitory para 
//poder trabajar con el service Bus en OrderRepository
//Vid156
var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionBuilder.UseSqlServer(connectionString);
builder.Services.AddSingleton(new EmailRepository(optionBuilder.Options));


//Vid156 Instanciamos los metodos del azure service bus
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAzureServiceBusConsumer();

app.Run();
