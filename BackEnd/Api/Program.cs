using Api.Application;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Infrastructure.Persistence;
using Api.Infrastructure.Persistence.Repositories;
using Api.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Logging definitions
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

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
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
