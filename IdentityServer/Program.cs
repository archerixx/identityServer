using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Models.Helper;
using Project.Domain.Models.Identity;
using Project.IdentityServer.IdentitySpecific;
using Project.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up..");

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

#region Database connection
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
}).AddEntityFrameworkStores<Context>();
#endregion

#region IdentityServer
builder.Services.AddScoped<IProfileService, CustomProfileService>();

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.EmitStaticAudienceClaim = true;
})
.AddOperationalStore(options => // enables PersistedGrantStore
{
    options.ConfigureDbContext = builder =>
        builder.UseSqlServer(connectionString,
            sql => sql.MigrationsAssembly(migrationsAssemly));

    // this enables automatic token cleanup. this is optional.
    options.EnableTokenCleanup = true;
    options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
})
.AddProfileService<CustomProfileService>()
.AddInMemoryClients(Config.Clients)
.AddInMemoryIdentityResources(Config.IdentityResources)
.AddInMemoryApiResources(Config.ApiResources) //configuration.GetSection("IdentityServer:ApiResource") // unable to load from app.config since secret is required
.AddInMemoryApiScopes(configuration.GetSection("IdentityServer:ApiScopes"))
.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
.AddSigningCredential(new X509Certificate2(configuration.GetSection("Certificate:PathTo").Value,
                configuration.GetSection("Certificate:Pass").Value)); // machine certificate needed - for developing DeveloperSingingCredential

builder.Services.AddScoped<UserClaimsPrincipalFactory<Users, Roles>>();
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

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>(); // for global exception handling - model can be modified to return needed Error type

// for seeding users
if (args.Contains("/seed"))
{
    Log.Information("Seeding database...");
    _ = new SeedDatabase(app.Services.GetRequiredService<IServiceScopeFactory>());
    Log.Information("Done seeding. Exiting");
    return;
}

app.UseIdentityServer();
app.UseHttpsRedirection();

app.MapGet("/", () => "Welcome to IdentityServer");
app.Run();
