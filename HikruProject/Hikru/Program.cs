using AutoMapper;
using FluentValidation;
using HikruCodeChallenge.Application.DTOs;
using HikruCodeChallenge.Application.Interfaces;
using HikruCodeChallenge.Application.Mappings;
using HikruCodeChallenge.Application.Validators;
using HikruCodeChallenge.Infrastructure.Data;
using HikruCodeChallenge.Infrastructure.Repositories;
using HikruCodeChallenge.Infrastructure.Services;
using HikruCodeChallenge.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database configuration for Oracle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// FluentValidation
builder.Services.AddScoped<IValidator<CreateAssessmentDto>, CreateAssessmentValidator>();
builder.Services.AddScoped<IValidator<UpdateAssessmentDto>, UpdateAssessmentValidator>();
builder.Services.AddScoped<IValidator<AssessmentFilterDto>, AssessmentFilterValidator>();

// Repository pattern
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hikru Assessment Management API",
        Version = "v1",
        Description = "A comprehensive API for managing assessments with CRUD operations, pagination, filtering, and authentication.",
        Contact = new OpenApiContact
        {
            Name = "Hikru Development Team",
            Email = "dev@hikru.com"
        }
    });

    // Configure API Key authentication
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. Use 'hikru-api-key-2025' as the API key.",
        In = ParameterLocation.Header,
        Name = "X-API-Key",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "ApiKey",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
};
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hikru Assessment API v1");
    c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger path
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();
});

// Add custom middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ensure database is created (for development)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Could not ensure database creation. Make sure Oracle database is running and connection string is correct.");
    }
}

app.Run();
