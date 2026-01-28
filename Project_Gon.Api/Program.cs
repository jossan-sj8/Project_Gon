using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project_Gon.Infrastructure.Data;
using Project_Gon.Infrastructure.Mappings;
using Project_Gon.Infrastructure.Repositories;
using Project_Gon.Infrastructure.Services;
using Serilog;
using FluentValidation;                    
using FluentValidation.AspNetCore;         
using Project_Gon.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ==================== LOGGING ====================
// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ==================== DATABASE ====================
// Agregar DbContext con PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==================== DEPENDENCY INJECTION ====================
// AutoMapper
builder.Services.AddAutoMapper(typeof(EmpresaMappingProfile).Assembly);

// Repository Pattern + Unit of Work
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ==================== PASSWORD HASHING ====================
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

// ==================== CORS ====================
// Configurar CORS para Angular (localhost:4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",      // Angular dev
            "http://localhost:80",        // Local server
            "https://localhost:443",      // Local HTTPS
            "http://127.0.0.1:4200"       // Alternative localhost
        )
        .AllowAnyMethod()                 // GET, POST, PUT, DELETE, etc.
        .AllowAnyHeader()                 // Content-Type, Authorization, etc.
        .AllowCredentials()               // Permitir cookies/auth headers
        .WithExposedHeaders("X-Total-Count", "X-Total-Pages"); // Headers personalizados
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ==================== JWT AUTHENTICATION ====================
// Configuración JWT con múltiples fuentes y validación
var jwtSection = builder.Configuration.GetSection("JwtSettings");

// Prioridad: 1. Variable de entorno, 2. Configuración, 3. Generación automática para dev
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? jwtSection.GetValue<string>("Secret")
    ?? GenerateDevJwtSecret(builder.Environment);

var jwtIssuer = jwtSection.GetValue<string>("Issuer") ?? "ProjectGon";
var jwtAudience = jwtSection.GetValue<string>("Audience") ?? "ProjectGon.Users";
var jwtExpirationMinutes = jwtSection.GetValue<int>("ExpirationMinutes", 60);

// Validar que el secret tenga la longitud mínima requerida
if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Secret debe tener al menos 32 caracteres para ser seguro. " +
        "Configure JWT_SECRET como variable de entorno o en appsettings.");
}

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // No tolerancia de tiempo
        RequireExpirationTime = true,
        RequireSignedTokens = true
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var userId = context.Principal?.FindFirst("sub")?.Value ?? "Unknown";
            var empresaId = context.Principal?.FindFirst("empresaId")?.Value ?? "Unknown";
            Log.Information("JWT Token validated for user: {UserId}, empresa: {EmpresaId}",
                userId, empresaId);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Log.Warning("JWT Challenge triggered: {Error}", context.Error ?? "Unknown");
            return Task.CompletedTask;
        }
    };
});

// Método auxiliar para generar secret en desarrollo
static string GenerateDevJwtSecret(IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        Log.Warning("Usando JWT Secret generado automáticamente para DESARROLLO. " +
                   "NO usar en producción. Configure JWT_SECRET como variable de entorno.");
        return "ProjectGon2026DevAutoGeneratedSecretKey_" +
               Convert.ToBase64String(Guid.NewGuid().ToByteArray()) +
               Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    throw new InvalidOperationException(
        "JWT Secret no configurado. Configure JWT_SECRET como variable de entorno.");
}

// ==================== SWAGGER / OPENAPI ====================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Gon API",
        Version = "v1",
        Description = "API REST para gestión de stock y ventas - SaaS",
        Contact = new OpenApiContact
        {
            Name = "Project Gon Support",
            Email = "support@project-gon.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT"
        }
    });

    // Agregar seguridad JWT al Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header usando Bearer scheme",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    // XML comentarios (opcional)
    var xmlFile = "Project_Gon.Api.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// ==================== SIGNALR ====================
// Para notificaciones en tiempo real
builder.Services.AddSignalR();

// ==================== CONTROLLERS ====================
builder.Services.AddControllers();

// ==================== FLUENTVALIDATION ====================
// FluentValidation para validaciones de DTOs
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ==================== HEALTH CHECKS ====================
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// ==================== BUILD ====================
var app = builder.Build();

// ==================== MIDDLEWARE ====================
// Configurar el pipeline de solicitudes HTTP

// Aplicar migraciones automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            dbContext.Database.Migrate();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error applying database migrations");
        }
    }

    // Swagger UI en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Gon API v1");
        c.RoutePrefix = "swagger"; // Acceder a http://localhost:5000/swagger
    });
}

// HTTPS Redirection
app.UseHttpsRedirection();

// Usar CORS (antes que autenticación)
app.UseCors("AllowAngular");

// Logging de requests HTTP
app.UseSerilogRequestLogging();

// Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// Health Checks endpoint
app.MapHealthChecks("/health");

// Controllers
app.MapControllers();

// SignalR Hubs
app.MapHub<VentasHub>("/hub/ventas");

// Endpoint por defecto (info de la API)
app.MapGet("/", () => new
{
    message = "Project Gon API v1",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow,
    documentation = "/swagger"
}).WithName("GetInfo").WithOpenApi();

Log.Information("Project Gon API iniciada en ambiente: {Environment}", app.Environment.EnvironmentName);

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicación terminó inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}