using MinimalApiDemo.Data;
using MinimalApiDemo.Endpoints;
using MinimalApiDemo.Settings;

var builder = WebApplication.CreateBuilder(args);

// Bind AppSettings section from configuration (combines appsettings.json and environment variables)
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

// Register dependencies and services
builder.Services.AddSingleton<ProductStore>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Handle exceptions in non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

// Enabled for all environments so you can test Swagger while in Production mode
app.UseSwagger();
app.UseSwaggerUI();

// Root health check endpoint
app.MapGet("/", () => "Minimal API is running");

// Register your modular endpoint groups
app.MapInfoEndpoints();
app.MapProductEndpoints();

app.Run();