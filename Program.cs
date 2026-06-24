using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VideoGameCharacter.Data;
using VideoGameCharacter.Services;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllers();

//Generates a JSON document that describes all my API endpoints, their parameters, and their responses-> /openapi/v1.json.
builder.Services.AddOpenApi();

//Registers VideoGameCharacterService as a scoped service, binding it to IVideoGameCharacterService automatically.
//With Scoped, a new instance is created per HTTP request and injected wherever the interface is used.
builder.Services.AddScoped<IVideoGameCharacterService, VideoGameCharacterService>();

//Registers AppDbContext as a service, connects it to SQL Server using the connection string ("DefaultConnection") defined in appsettings.json file.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Creates the web application.
var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //Serves the generated OpenAPI spec as an endpoint at /openapi/v1.json.
    app.MapOpenApi();
    
    //Serves the Scalar UI at /scalar/v1.
    //Reads the JSON from /openapi/v1.json and renders it into the interactive documentation page that is seen in the browser.
    app.MapScalarApiReference();
}

//Automatically redirects any HTTP request to HTTPS.
app.UseHttpsRedirection();

//Enables the authorization middleware in the request pipeline. Checks whether the user has permission to access a given endpoint.
app.UseAuthorization();

//Scans the project for all classes that inherit from ControllerBase and registers their endpoints into the routing system.
app.MapControllers();

//Starts the web server and blocks the thread, keeping the app alive and listening for incoming HTTP requests.
app.Run();