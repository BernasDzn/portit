using Api.Application.Services.Auth.Providers;
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
using Prometheus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Infrastructure.Filters;

var builder = WebApplication.CreateBuilder(args);

// Check if we're in Testing environment (skip auth/authz and use in-memory database)
var isTestingEnvironment = builder.Environment.IsEnvironment("Testing");
if (isTestingEnvironment)
{
    Console.WriteLine("WARNING: Running in TESTING ENVIRONMENT - Authentication and Authorization are DISABLED!");
    Console.WriteLine("WARNING: Using IN-MEMORY DATABASE - All data will be lost on restart!");
}

// Logging definitions
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Filter.ByExcluding(logEvent => 
    {
        // Filter out Serilog request logging for /metrics
        if (logEvent.Properties.ContainsKey("RequestPath") && 
            logEvent.Properties["RequestPath"].ToString().Contains("/metrics"))
            return true;
        
        // Filter out ASP.NET Core logs for /metrics endpoint
        if (logEvent.Properties.ContainsKey("Name") && 
            logEvent.Properties["Name"].ToString().Contains("Prometheus metrics"))
            return true;
        
        // Filter out generic request logs that mention /metrics in the message
        if (logEvent.MessageTemplate.Text.Contains("Request") && 
            logEvent.RenderMessage().Contains("/metrics"))
            return true;
        
        return false;
    }));

// Enable HTTPS
/*
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(builder.Configuration.GetValue<int>("http_port"));
    options.ListenLocalhost(builder.Configuration.GetValue<int>("https_port"), listenOptions =>
    {
        listenOptions.UseHttps();
    });
});
*/

// Allow requests from one or more frontend origins (Vue dev server, hosted frontends, ...)
// Read an array from configuration (frontend_urls). Fall back to single frontend_url for backward compatibility.
var frontendUrls = builder.Configuration.GetSection("frontend_urls").Get<string[]>() ?? new[] { builder.Configuration.GetValue<string>("frontend_url")! };

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDevPolicy", policy =>
    {
        policy.WithOrigins(frontendUrls)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Authentication (skip if Testing environment)
if (!isTestingEnvironment)
{
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
        ValidIssuer = builder.Configuration.GetValue<string>("backend_url")!,
        ValidateAudience = true, // Require that token aud claim matches configured audience (our front-end)
        // Accept any of the configured frontend URLs as valid audiences for tokens
        ValidAudiences = frontendUrls,
        ValidateLifetime = true, // Ensure token hasn't expired
        ValidateIssuerSigningKey = true, // Ensure token signature is valid so it cant be forged
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        RoleClaimType = "user_role", // Map the role claim type used when creating tokens.
        ClockSkew = TimeSpan.FromMinutes(2) // Allows for a small time difference between server and client
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // if there is no authorization header try to read token from cookie
            if (string.IsNullOrEmpty(context.Token))
            {
                if (context.Request.Cookies.TryGetValue("AuthToken", out var cookieToken) &&
                    !string.IsNullOrEmpty(cookieToken))
                {
                    context.Token = cookieToken;
                }
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Debug: Log the claims in the token
            var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
            if (claims != null)
            {
                Console.WriteLine("JWT Token Claims:");
                foreach (var claim in claims)
                {
                    Console.WriteLine($"  {claim}");
                }
            }
            return Task.CompletedTask;
        }
    };
})
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

    builder.Services.AddAuthorization(options =>
{
    // Authorization step, after identification of the user, we want to know what they can access
    // This policy just requires that the user is authenticated
    // It will be used for the "me" endpoint
    options.AddPolicy("ApiUser", policy => policy.RequireAuthenticatedUser());
    // Port Authority Officer features
    options.AddPolicy("VesselType.Manage", p => p.RequireRole("PortAuthorityOfficer", "Administrator"));
    options.AddPolicy("Vessel.Manage", p => p.RequireRole("PortAuthorityOfficer", "Administrator"));
    options.AddPolicy("Vessel.View", p => p.RequireRole("SAORepresentative", "PortAuthorityOfficer", "Administrator"));
    options.AddPolicy("StorageArea.Manage", p => p.RequireRole("PortAuthorityOfficer", "Administrator"));
    options.AddPolicy("ShippingAgentOrg.Manage", p => p.RequireRole("PortAuthorityOfficer", "Administrator"));
    options.AddPolicy("Representative.Manage", p => p.RequireRole("SAORepresentative", "PortAuthorityOfficer", "Administrator"));
    options.AddPolicy("Dock.Manage", p => p.RequireRole("PortAuthorityOfficer", "Administrator"));
    // Vessel visit notification: 
    // - viewing by SAORepresentative and PortAuthorityOfficer
    // - edits/submissions by SAORepresentative, 
    // - decisions by PortAuthorityOfficer
    // TODO: refine these policies since VVNs are complicated...
    options.AddPolicy("VesselVisitNotification.View", p => p.RequireRole("SAORepresentative", "PortAuthorityOfficer", "Administrator"));
    options.AddPolicy("VesselVisitNotification.Edit", p => p.RequireRole("SAORepresentative", "Administrator"));
    options.AddPolicy("VesselVisitNotification.Approve", p => p.RequireRole("PortAuthorityOfficer", "Administrator"));
    options.AddPolicy("VesselVisitNotification.Submit", p => p.RequireRole("SAORepresentative", "Administrator"));
    // Logistics Operator features
    options.AddPolicy("Qualification.Manage", p => p.RequireRole("LogisticsOperator", "Administrator"));
    options.AddPolicy("Staff.Manage", p => p.RequireRole("LogisticsOperator", "Administrator"));
    options.AddPolicy("PhysicalResource.Manage", p => p.RequireRole("LogisticsOperator", "Administrator"));
        // Admin-only fallback for the rest of the features
        options.AddPolicy("AdminOnly", p => p.RequireRole("Administrator"));
    });
}
else
{
    // When skipping auth/authz, add all policies but with no requirements (allow all)
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ApiUser", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("VesselType.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("Vessel.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("Vessel.View", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("StorageArea.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("ShippingAgentOrg.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("Representative.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("Dock.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("VesselVisitNotification.View", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("VesselVisitNotification.Edit", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("VesselVisitNotification.Approve", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("VesselVisitNotification.Submit", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("Qualification.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("Staff.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("PhysicalResource.Manage", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(_ => true));
    });
}

// Set encryption key for the application
EncryptionHelper.SetEncryptionKey(builder.Configuration["EncryptionKey"]!);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    // Safeguard validation errors from leaking implementation details (and to help the UI not display insane logs)
    options.Filters.Add(new GlobalExceptionFilter());
})
.ConfigureApiBehaviorOptions(options =>
{
    // Override automatic ModelState validation response
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => new
            {
                Field = e.Key,
                Error = e.Value!.Errors.First().ErrorMessage
            })
            .ToList();

        return new BadRequestObjectResult(new
        {
            Message = "Validation failed",
            Errors = errors
        });
    };
});;
IConfiguration configuration = builder.Configuration;

// Register authentication providers
builder.Services.AddTransient<IAuthProvider, GoogleAuthProvider>();
//builder.Services.AddTransient<IAuthProvider, OtherAuthProvider>();

// Register IJwtTokenService with config-based factory
builder.Services.AddTransient<IJwtTokenService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new JwtTokenService(
        config["Jwt:Key"]!,
        config["backend_url"]!,
        config["frontend_url"]!,
        config.GetValue<int>("Jwt:ExpiresMinutes")
    );
});

// Add database contexts
if (isTestingEnvironment)
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

// Configure ASP.NET Core Identity (using IdentityCore to avoid cookie-based authentication)
builder.Services.AddIdentityCore<SystemUser>(options =>
{
    // Password settings (adjust as needed - we don't use passwords directly since we use Google OAuth)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddRoles<SystemUserRole>()
.AddEntityFrameworkStores<ApiContext>();

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
builder.Services.AddTransient<IAdminService, AdminService>();

var app = builder.Build();

app.Logger.LogInformation("Starting application");
app.Logger.LogInformation("Environment: {EnvironmentName}", app.Environment.EnvironmentName);

// Always bootstrap in Testing environment, or when explicitly configured
if (isTestingEnvironment || configuration.GetValue<bool>("NukeDatabaseAndRunBootstrap"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApiContext>();
        var userManager = services.GetRequiredService<UserManager<SystemUser>>();
        var roleManager = services.GetRequiredService<RoleManager<SystemUserRole>>();
    
        await Bootstrap.InitAsync(context, userManager, roleManager, nukeDatabase: true);
        
        if (isTestingEnvironment)
        {
            app.Logger.LogInformation("✓ Bootstrap completed for Testing environment");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || isTestingEnvironment)
{
    app.UseOpenApi();
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

//app.UseHttpsRedirection();
app.UseCors("VueDevPolicy");

if (!isTestingEnvironment)
{
    app.UseAuthentication();
    app.UseAuthorization();
}
else
{
    app.Logger.LogWarning("⚠️  Authentication and Authorization middleware are DISABLED!");
}

// Prometheus metrics endpoint for Grafana
app.UseHttpMetrics();  // Collects HTTP request metrics (duration, count, etc.)

app.MapControllers();
app.MapMetrics();      // Exposes /metrics endpoint at http://localhost:2226/metrics

app.Run();

public partial class Program { }
