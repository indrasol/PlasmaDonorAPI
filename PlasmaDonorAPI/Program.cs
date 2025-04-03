using Microsoft.EntityFrameworkCore;
using NewPlasmaDonorsAPI.Data;
using NewPlasmaDonorsAPI.Repositories;
using NewPlasmaDonorsAPI.Services;
using NewPlasmaDonorsAPI.Startup;
using NewPlasmaDonorsAPI.utils;
using PlasmaDonorAPI.Repositories;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using NewPlasmaDonorsAPI.Services.excel;
using NewPlasmaDonorsAPI.Services.Validator;

var builder = WebApplication.CreateBuilder(args);


// Add configuration files
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);

//.AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.PropertyNamingPolicy = null;
//});

// OpenAPI configuration
builder.Services.AddOpenApi();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Enable API explorer for multiple versions
//builder.Services.AddVersionedApiExplorer(options =>
//{
//    options.GroupNameFormat = "'v'VVV";
//    options.SubstituteApiVersionInUrl = true;
//});

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
builder.Services.AddScoped<ExcelProcessor>();
builder.Services.AddScoped<ProfileValidator>(); // Required dependency
builder.Services.AddScoped<StatRepo>();
builder.Services.AddScoped<IStatRepo, StatRepo>();
builder.Services.AddScoped<StatService>();
builder.Services.AddScoped<TreeUtils>();
builder.Services.AddScoped<NewPlasmaDonorsAPI.Services.excel.ExcelProcessor>();
builder.Services.AddScoped<SqlUtilService>();
builder.Services.AddScoped<NewPlasmaDonorsAPI.Services.Validator.ProfileValidator>();
builder.Services.AddScoped<BaseService>();
builder.Services.AddScoped<ILogger<ProfileImportService>, Logger<ProfileImportService>>();
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

// Add IIS integration
builder.Services.Configure<IISOptions>(options =>
{
    options.AutomaticAuthentication = false;
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

// Add Routing explicitly
app.UseRouting();


app.UseCors("AllowAll");
app.UseHttpsRedirection();
//app.UseCors(MyAllowSpecificOrigins);



// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Plasma Donor API";
    });

}

//// Ensure endpoints are mapped correctly
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

// Map controllers
app.MapControllers();

// Run the application
app.Run();
