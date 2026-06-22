using BusinessLogicLayer;
using CommerceFabric.ProductService.API.Middleware;
using DataAccessLayer;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add my project services to the container.
builder.Services.AddDataAccessLayer();
builder.Services.AddBusinessLogicLayer();

// Add Controllers to the container.
builder.Services.AddControllers();

//FluentValidation
builder.Services.AddFluentValidationAutoValidation();

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline using the custom exception handling middleware.
app.UseExceptionHandlingMiddleware();
app.UseRouting();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
