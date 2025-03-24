using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Repositories;
using NewPlasmaDonorsAPI.Services;
using NewPlasmaDonorsAPI.Startup;
using NewPlasmaDonorsAPI.utils;
using PlasmaDonorAPI.Repositories;
using IoC;

var builder = WebApplication.CreateBuilder(args);

// Add configuration files
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// OpenAPI configuration
builder.Services.AddOpenApi();

// Get the JWT secret from the configuration
var jwtSecret = builder.Configuration["JwtSecret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JwtSecret is not configured in appsettings.json or environment variables.");
}

// Configure MySQL connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register repositories and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICompanyLocationRepository, CompanyLocationRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ProfileRepository>();
builder.Services.AddScoped<DonarInfluencerMapRepository>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<MdService>();
builder.Services.AddSingleton<IJwtService, JwtService>(_ => new JwtService(jwtSecret));
builder.Services.AddScoped<MdRepository>();
builder.Services.AddScoped<ProfileImportService>();
builder.Services.AddScoped<StatRepo>();
builder.Services.AddScoped<IStatRepo, StatRepo>();
builder.Services.AddScoped<StatService>();
builder.Services.AddScoped<TreeUtils>();
builder.Services.AddScoped<NewPlasmaDonorsAPI.Services.excel.ExcelProcessor>();
builder.Services.AddScoped<SqlUtilService>();
builder.Services.AddScoped<NewPlasmaDonorsAPI.Services.Validator.ProfileValidator>();
builder.Services.AddScoped<BaseService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// JWT and authentication services
builder.Services.AddAuthConfig(jwtSecret);

//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Allow Angular frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
});
});

// Build the application
var app = builder.Build();

// Add this middleware for debugging purposes if needed
//app.Use(async (context, next) =>
//{
//    if (context.Request.Method == "OPTIONS")
//    {
//        context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
//        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
//        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
//        context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
//        context.Response.StatusCode = 200;
//        return;  // Don't call the next middleware for OPTIONS request
//    }
//    await next.Invoke(); // Call the next middleware for other requests
//});


app.UseCors("AllowAll");
app.UseHttpsRedirection();
//app.UseCors(MyAllowSpecificOrigins);

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  
}

// Map controllers
app.MapControllers();

// Run the application
app.Run();
