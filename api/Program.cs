using Api.Domain.Model;
using Application.Services;
using DAL;
using DataModel.Repository;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddTransient<VesselService>();

var app = builder.Build();

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
