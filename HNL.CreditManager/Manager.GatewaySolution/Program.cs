using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);



//Vid 161.2 Add Ocelot to the Gateway
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:44349/";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };



    });


builder.Services.AddOcelot();


var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//Vid 161.3 Add Ocelot to the Gateway
await app.UseOcelot();

app.Run();
