using BusinessLogicLayer;
using CommerceFabric.ProductService.API.ApiEndpoints;
using CommerceFabric.ProductService.API.Middleware;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add my project services to the container.
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

// Required to allow for enums as strings in the DTOs passed to the Minimal API Endpoints
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter()
    );
});

// Add Controllers to the container.
builder.Services.AddControllers();

//FluentValidation
builder.Services.AddFluentValidationAutoValidation();

// Add swagger generation to create API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Cors Services
builder.Services.AddCors(options =>
{
    // Configure a default CORS policy that allows any origin, method, and header
    options.AddDefaultPolicy(builder =>
    {
        // todo - may want to restrict this in production to only allow specific origins, methods, and headers
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline using the custom exception handling middleware.
app.UseExceptionHandlingMiddleware();
app.UseRouting();

// Enable swagger UI and endpoint for API documentation
app.UseSwagger(); // adds endpoint that can serve the swagger.json
app.UseSwaggerUI(); // adds swagger UI to visualize and interact with the API's resources
app.UseCors(); // Enable Cross-Origin Resource Sharing (CORS) using the configured policy, controlling which origins, methods, and headers can access this API.

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Map the Minimal API Endpoints
app.MapProductApiEndpoints();

app.Run();
