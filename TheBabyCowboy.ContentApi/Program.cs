
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowBoy.ContentApi.Services;
using TheBabyCowBoy.ContentApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//    {
//        Title = "TheBabyCowBoy.ContentApi",
//        Version = "v1"
//    });

//    // Temporarily use Swagger 2.0 for compatibility testing.
//});


// PostgreSQL
var connectionString =
    builder.Configuration.GetConnectionString("ContentDatabase");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'ContentDatabase' is not configured.");
}

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(connectionString));

// CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://127.0.0.1:5173",
                "https://127.0.0.1:5173",
                "https://YOUR-LIVE-FRONTEND-DOMAIN"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillCategoryService, SkillCategoryService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<ITravelService, TravelService>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "TheBabyCowboy.ContentApi v1");
});

app.UseHttpsRedirection();

// CORS must run before authorization
app.UseCors("ReactFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
