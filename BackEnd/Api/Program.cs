using Api.Application;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Infrastructure.Persistence;
using Api.Infrastructure.Persistence.Repositories;
using Api.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using ZstdSharp.Unsafe;
using NSwag;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using NSwag.Generation.Processors.Security;
using Api.Infrastructure.Utilities.Email;

var builder = WebApplication.CreateBuilder(args);
// Logging definitions
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration));

// Enable HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5195);
    options.ListenLocalhost(builder.Configuration.GetValue<int>("https_port"), listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// Allow all requests from Vue dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDevPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// JwtBearer for API authorization
.AddJwtBearer(options =>
{
    // Define validation parameters for Bearer token headers
    // When an HTTP request with a Bearer token is received, these parameters are used to validate the token
    // If it passes the HttpContext.User will be populated with the token claims
    // If it fails a 401 Unauthorized response is returned automatically
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Require that token iss claim matches configured issuer (us)
        ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer")!, 
        ValidateAudience = true, // Require that token aud claim matches configured audience (our front-end)
        ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience")!,
        ValidateLifetime = true, // Ensure token hasn't expired
        ValidateIssuerSigningKey = true, // Ensure token signature is valid so it cant be forged
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.FromMinutes(2) // Allows for a small time difference between server and client
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

// Authorization step, after identification of the user, we want to know what they can access
// This policy just requires that the user is authenticated
// It will be used for the "me" endpoint
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiUser", policy => policy.RequireAuthenticatedUser());
});

// Set encryption key for the application
EncryptionHelper.SetEncryptionKey(builder.Configuration["EncryptionKey"]!);

// Add services to the container.
builder.Services.AddControllers();
IConfiguration configuration = builder.Configuration;

// Add database contexts
if(builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<ApiContext>(opt =>
        opt.UseLazyLoadingProxies().UseInMemoryDatabase("TestDatabase"));
}
else
{
    builder.Services.AddDbContext<ApiContext>(opt =>
        opt.UseLazyLoadingProxies().UseMySQL(
            $"server={configuration["DatabaseSettings:server"]};port={configuration["DatabaseSettings:port"]};database={configuration["DatabaseSettings:database"]};user={configuration["DatabaseSettings:username"]};password={configuration["DatabaseSettings:password"]}"
        ));
}

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTransient<IQualificationRepository, QualificationRepository>();
builder.Services.AddTransient<IQualificationService, QualificationService>();
builder.Services.AddTransient<IDockRepository, DockRepository>();
builder.Services.AddTransient<IDockService,DockService>();
builder.Services.AddTransient<IVesselTypeRepository, VesselTypeRepository>();
builder.Services.AddTransient<IVesselTypeService, VesselTypeService>();
builder.Services.AddTransient<IVesselRepository, VesselRepository>();
builder.Services.AddTransient<IShippingAgentOrgRepository, ShippingAgentOrgRepository>();
builder.Services.AddTransient<IRepresentativeRepository, RepresentativeRepository>();
builder.Services.AddTransient<RepresentativeService>();
builder.Services.AddTransient<IVesselService, VesselService>();
builder.Services.AddTransient<IStorageAreaRepository, StorageAreaRepository>();
builder.Services.AddTransient<IStorageAreaService,StorageAreaService>();
builder.Services.AddTransient<IPhysicalResourceRepository, PhysicalResourceRepository>();
builder.Services.AddTransient<IPhysicalResourceService, PhysicalResourceService>();
builder.Services.AddTransient<IStaffRepository, StaffRepository>();
builder.Services.AddTransient<IStaffService, StaffService>();
builder.Services.AddTransient<IContainerRepository, ContainerRepository>();
builder.Services.AddTransient<IVesselVisitNotificationRepository, VesselVisitNotificationRepository>();
builder.Services.AddTransient<IVesselVisitNotificationService, VesselVisitNotificationService>();
builder.Services.AddTransient<VesselVisitNotificationIdGenerator>();
builder.Services.AddTransient<INotificationDecisionService, NotificationDecisionService>();
builder.Services.AddTransient<ISystemUserRepository, SystemUserRepository>();
builder.Services.AddTransient<ISystemUserService, SystemUserService>();
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

var app = builder.Build();

app.Logger.LogInformation("Starting application");
app.Logger.LogInformation("Environment: {EnvironmentName}", app.Environment.EnvironmentName);

if (configuration.GetValue<bool>("NukeDatabaseAndRunBootstrap"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApiContext>();
    
        Bootstrap.Init(context, nukeDatabase: true);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();
app.UseCors("VueDevPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
