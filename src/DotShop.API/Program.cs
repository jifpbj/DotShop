// ============================================================
// Program.cs — Application Entry Point and Composition Root
// ============================================================
// This file does three things:
//   1. Registers all services into the DI container (builder.Services.*)
//   2. Builds the app (builder.Build())
//   3. Configures the middleware pipeline (app.Use*) and starts the server
//
// Think of it as the "wiring closet" — every component is connected here,
// but the actual logic lives in its own class.
// ============================================================

using System.Text;
using DotShop.API;
using DotShop.API.Middleware;
using DotShop.API.Services;
using DotShop.API.Services.Interfaces;
using DotShop.Data;
using DotShop.Data.Configuration;
using DotShop.Data.Repositories;
using DotShop.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===========================================================
// SECTION 1 — Configuration Binding
// ===========================================================
// Tell .NET to read the "MongoDbSettings" and "JwtSettings" sections
// from appsettings.json and bind them to our typed setting classes.
// Any class that needs these values declares IOptions<T> in its constructor
// and .NET fills them in automatically.
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// ===========================================================
// SECTION 2 — Database (MongoDB)
// ===========================================================
// MongoDbContext is the wrapper around the MongoDB connection pool.
// Singleton = one instance shared by all requests — correct because
// MongoClient is thread-safe by design.
builder.Services.AddSingleton<MongoDbContext>();

// ===========================================================
// SECTION 3 — Repositories
// ===========================================================
// Scoped = one instance per HTTP request.
// Each request gets a fresh repository but they all share the same
// MongoDbContext (and therefore the same connection pool).
//
// We register the interface → implementation pair so any class
// that asks for IProductRepository receives a ProductRepository.
builder.Services.AddScoped<IProductRepository,  ProductRepository>();
builder.Services.AddScoped<IOrderRepository,    OrderRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// ===========================================================
// SECTION 4 — Services
// ===========================================================
builder.Services.AddScoped<IProductService,   ProductService>();
builder.Services.AddScoped<IOrderService,     OrderService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ICustomerService,  CustomerService>();
builder.Services.AddScoped<IAuthService,      AuthService>();

// ===========================================================
// SECTION 5 — JWT Authentication
// ===========================================================
// Read the JWT settings so we can reference them during setup.
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;

// AddAuthentication registers the auth system with JWT as the default scheme.
// AddJwtBearer configures how incoming tokens are validated:
//   - Is the issuer correct?      (matches our API's name)
//   - Is the audience correct?    (matches our Angular client)
//   - Has the token expired?
//   - Is the signature valid?     (proves we created it with our SecretKey)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings.Issuer,
            ValidAudience            = jwtSettings.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

// ===========================================================
// SECTION 6 — Controllers
// ===========================================================
// AddControllers registers all classes annotated with [ApiController]
// as request handlers. It also enables model validation via data annotations
// (the [Required], [Range] etc. on our DTOs).
builder.Services.AddControllers();

// ===========================================================
// SECTION 7 — Swagger / OpenAPI
// ===========================================================
// Swagger generates an interactive API documentation page at /swagger.
// You can test every endpoint from the browser — extremely useful for demos.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "DotShop API",
        Version     = "v1",
        Description = "Demo electronics remarketing platform — Magnakom interview project"
    });

    // Add a JWT "Authorize" button to the Swagger UI.
    // This lets you paste a token once and have it sent with every test request.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat = "JWT",
        Description = "Paste your JWT token here. Obtained from POST /api/auth/login."
    });

    // Tells Swagger to include the Authorization header on all secured endpoints.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===========================================================
// SECTION 8 — CORS (Cross-Origin Resource Sharing)
// ===========================================================
// Browsers block requests from one origin (http://localhost:4200)
// to a different origin (http://localhost:5047) by default.
// CORS tells the browser that our API intentionally allows this.
//
// "ReactDev" policy allows the Angular dev server to call the API.
// In production you'd replace this with your actual domain.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDev", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ===========================================================
// Build the app — no more service registrations after this line
// ===========================================================
var app = builder.Build();

// ===========================================================
// SECTION 9 — Middleware Pipeline
// ===========================================================
// ORDER MATTERS. Each middleware wraps the next like Russian nesting dolls.
// A request flows IN through each layer; the response flows back OUT.

// Global error handler — catches unhandled exceptions anywhere in the pipeline
// and converts them to consistent JSON responses. Must be FIRST so it catches
// errors thrown by all subsequent middleware and controllers.
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Serve Swagger JSON at /swagger/v1/swagger.json
app.UseSwagger();
// Serve the interactive Swagger UI at /swagger
app.UseSwaggerUI();

// Allow the Angular dev server to make requests.
app.UseCors("AngularDev");

// Seed the database with sample products if running in Development mode
// and the products collection is empty.
if (app.Environment.IsDevelopment())
    await SeedData.SeedAsync(app.Services);

// UseAuthentication reads the JWT from the Authorization header and
// populates HttpContext.User with the token's claims.
// Must come BEFORE UseAuthorization.
app.UseAuthentication();

// UseAuthorization checks [Authorize] attributes on controllers.
// If the user isn't authenticated or lacks the required role,
// it returns 401 Unauthorized or 403 Forbidden automatically.
app.UseAuthorization();

// Maps requests to the matching [ApiController] action method.
app.MapControllers();

app.Run();
