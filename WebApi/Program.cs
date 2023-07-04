using IdentityServer4.AccessTokenValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.Domain.Interfaces.Identity;
using Project.Domain.Models.Helper;
using Project.Domain.Models.Identity;
using Project.Infrastructure;
using Project.Infrastructure.Repositories.Identity;
using Project.Service.Identity;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

#region Database access
var connectionString = configuration.GetConnectionString("Connection");
var migrationsAssemly = typeof(Context).GetTypeInfo().Assembly.GetName().Name;

builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssemly));
});
builder.Services.AddIdentity<Users, Roles>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
}).AddEntityFrameworkStores<Context>();
#endregion

#region Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Modift to allow only certain Headers/Originis/Methods
        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
});
#endregion

#region Authentication
builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme,
        jwtOptions =>
        {
            // base-address of your identityserver
            jwtOptions.Authority = configuration.GetSection("IdentityServer:IssuerUri").Value;
            jwtOptions.Audience = configuration.GetSection("IdentityServer:Audience").Value;

            jwtOptions.TokenValidationParameters.ValidateIssuer = false;
            jwtOptions.TokenValidationParameters.ValidateLifetime = false;

            // audience is optional
            jwtOptions.TokenValidationParameters.ValidateAudience = false;

            // it's recommended to check the type header to avoid "JWT confusion" attacks
            jwtOptions.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };

            jwtOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                NameClaimType = "name",
                RoleClaimType = "role"
            };
        },
        referenceOptions =>
        {
            // oauth2 introspection options
            referenceOptions.Authority = configuration.GetSection("IdentityServer:IssuerUri").Value;

            referenceOptions.ClientId = configuration.GetSection("IdentityServer:Client:ClientId").Value;
            referenceOptions.ClientSecret = configuration.GetSection("IdentityServer:Client:ClientSecret").Value;

            referenceOptions.RoleClaimType = "role";
        });
#endregion

#region Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.MinimumLevel.Debug().MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning).MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
        theme: AnsiConsoleTheme.Code).Enrich.FromLogContext();
});
#endregion

#region Scoped services
//builder.Services.AddScoped<IUserClaimsPrincipalFactory<Users>, UserClaimsPrincipalFactory<Users>>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>(); // for global exception handling - model can be modified to return needed Error type

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
