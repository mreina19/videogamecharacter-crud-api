using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using VideoGameCharacter.Data;
using VideoGameCharacter.Services;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllers();

//Generates a JSON document that describes all my API endpoints, their parameters, and their responses-> /openapi/v1.json.
builder.Services.AddOpenApi();

//Registers VideoGameCharacterService and AuthService as scoped services, binding them to IVideoGameCharacterService and IAuthService, respectively.
//With Scoped, a new instance is created per HTTP request and injected wherever the interface is used.
builder.Services.AddScoped<IVideoGameCharacterService, VideoGameCharacterService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//Registers AppDbContext as a service, connects it to SQL Server using the connection string ("DefaultConnection") defined in appsettings.json file.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Configures JWT Bearer authentication. Tells the middleware how to validate an incoming token's signature, issuer, audience, and expiration using the settings in appsettings.json.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token")!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience"),
            ValidateLifetime = true
        };
    });

//Registers ASP.NET Core's authorization system as a service.
builder.Services.AddAuthorization();

//Registers DbClaimsTransformation, which re-checks each user's Role and IsActive status from the database on every authenticated request, rather than trusting the token alone.
builder.Services.AddScoped<IClaimsTransformation, DbClaimsTransformation>();

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

//Enables the authentication middleware. Reads the Authorization header, validates the JWT using the rules configured in AddJwtBearer, and reconstructs the caller's identity/claims.
app.UseAuthentication();

//Enables the authorization middleware in the request pipeline. Checks whether the user has permission to access a given endpoint.
app.UseAuthorization();

//Scans the project for all classes that inherit from ControllerBase and registers their endpoints into the routing system.
app.MapControllers();

//Starts the web server and blocks the thread, keeping the app alive and listening for incoming HTTP requests.
app.Run();