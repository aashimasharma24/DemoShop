using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;
using DemoShop.Manager.Repositories;
using DemoShop.Manager.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Asp.Versioning;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// 1. Configure Serilog Logging
// -------------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build())
    .CreateLogger();
builder.Host.UseSerilog();

// -------------------------
// 2. Add DB Context & EF Core
// -------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DemoShopDbContext>(options =>
    options.UseSqlServer(connectionString));

// -------------------------
// 3. Configure Redis Caching (optional)
// -------------------------
/** TODO **/

// -------------------------
// 4. Configure Repositories & Services
// -------------------------
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaymentService, MockPaymentService>();

// -------------------------
// 5. Configure Authentication & JWT
// -------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings.GetValue<string>("Key");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
});

// -------------------------
// 6. Configure API Versioning
// -------------------------
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();