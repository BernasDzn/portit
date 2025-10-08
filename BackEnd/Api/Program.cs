using Api.Application;
using Api.Application.Services;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Infrastructure.Persistence;
using Api.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Logging definitions
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();

// Add database contexts
builder.Services.AddDbContext<ApiContext>(opt =>
    opt.UseLazyLoadingProxies().UseInMemoryDatabase("database"));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTransient<IQualificationRepository, QualificationRepository>();
builder.Services.AddTransient<QualificationService>();
builder.Services.AddTransient<IDockRepository, DockRepository>();
builder.Services.AddTransient<DockService>();
builder.Services.AddTransient<IVesselTypeRepository, VesselTypeRepository>();
builder.Services.AddTransient<VesselTypeService>();
builder.Services.AddTransient<IVesselRepository, VesselRepository>();
builder.Services.AddTransient<IShippingAgentOrgRepository, ShippingAgentOrgRepository>();
builder.Services.AddTransient<IRepresentativeRepository, RepresentativeRepository>();
builder.Services.AddTransient<RepresentativeService>();
builder.Services.AddTransient<VesselService>();
builder.Services.AddTransient<IStorageAreaRepository, StorageAreaRepository>();
builder.Services.AddTransient<StorageAreaService>();
builder.Services.AddTransient<IPhysicalResourceRepository, PhysicalResourceRepository>();
builder.Services.AddTransient<PhysicalResourceService>();
builder.Services.AddTransient<IStaffRepository, StaffRepository>();
builder.Services.AddTransient<StaffService>();
builder.Services.AddTransient<IVesselVisitNotificationRepository, VesselVisitNotificationRepository>();
builder.Services.AddTransient<VesselVisitNotificationService>();
builder.Services.AddTransient<NotificationDecisionService>();

var app = builder.Build();

app.Logger.LogInformation("Starting application");
app.Logger.LogInformation("Environment: {EnvironmentName}", app.Environment.EnvironmentName);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApiContext>();

    Bootstrap.Init(context, nukeDatabase: true);
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
