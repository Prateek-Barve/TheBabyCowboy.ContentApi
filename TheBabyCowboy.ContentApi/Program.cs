
using Microsoft.EntityFrameworkCore;
using TheBabyCowboy.ContentApi.Data;
using TheBabyCowBoy.ContentApi.Services;
using TheBabyCowBoy.ContentApi.Services.Interfaces;
using Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
// Clerk authentication
var clerkIssuer = builder.Configuration["Clerk:Issuer"];
var allowedUserId = builder.Configuration["Clerk:AllowedUserId"];

if (string.IsNullOrWhiteSpace(clerkIssuer) ||
    string.IsNullOrWhiteSpace(allowedUserId))
{
    throw new InvalidOperationException(
        "Clerk:Issuer and Clerk:AllowedUserId must be configured.");
}

var authorizedParties =
    builder.Configuration
        .GetSection("Clerk:AuthorizedParties")
        .Get<string[]>()
    ?? Array.Empty<string>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = clerkIssuer;
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = clerkIssuer,

                // Clerk's default session token may not contain an audience.
                ValidateAudience = false,

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var azp = context.Principal?
                    .FindFirst("azp")?.Value;

                // If azp exists, it must match a configured frontend origin.
                if (!string.IsNullOrEmpty(azp) &&
                    !authorizedParties.Contains(
                        azp,
                        StringComparer.OrdinalIgnoreCase))
                {
                    context.Fail(
                        "The token was issued for an unauthorized frontend.");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();

        policy.RequireAssertion(context =>
            context.User.FindFirst("sub")?.Value == allowedUserId);
    });
});

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

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();

var dataSource = dataSourceBuilder.Build();

//builder.Services.AddDbContext<ContentDbContext>(options =>
//    options.UseNpgsql(connectionString));
builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(dataSource));

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
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IJournalService, JournalService>();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
