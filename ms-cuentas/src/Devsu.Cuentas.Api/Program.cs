using System.Text.Json.Serialization;
using Devsu.Cuentas.Api.Config;
using Devsu.Cuentas.Api.Web.Dto;
using Devsu.Cuentas.Application;
using Devsu.Cuentas.Infrastructure;
using Devsu.Cuentas.Infrastructure.Messaging;
using Devsu.Cuentas.Infrastructure.Persistence;
using Devsu.Cuentas.Infrastructure.Vault;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. Secretos de BD/RabbitMQ desde Vault (o configuracion si esta deshabilitado)
// ============================================================
var vaultOptions = builder.Configuration.GetSection(VaultOptions.SectionName).Get<VaultOptions>() ?? new VaultOptions();
vaultOptions.Address = Environment.GetEnvironmentVariable("VAULT_ADDR")
    ?? Environment.GetEnvironmentVariable("VAULT_URI") ?? vaultOptions.Address;
vaultOptions.Token = Environment.GetEnvironmentVariable("VAULT_TOKEN")
    ?? Environment.GetEnvironmentVariable("SPRING_CLOUD_VAULT_TOKEN") ?? vaultOptions.Token;
vaultOptions.Password = Environment.GetEnvironmentVariable("VAULT_USERPASS_PASSWORD") ?? vaultOptions.Password;

VaultSecrets secrets;
using (var loggerFactory = LoggerFactory.Create(b => b.AddConsole()))
{
    var startupLogger = loggerFactory.CreateLogger("Startup");
    if (vaultOptions.Enabled)
    {
        secrets = await VaultSecretsLoader.LoadAsync(vaultOptions, startupLogger);
    }
    else
    {
        startupLogger.LogWarning("[Vault] Deshabilitado: usando credenciales de configuracion.");
        var db = builder.Configuration.GetSection("Database");
        secrets = new VaultSecrets(
            db["Username"] ?? "devsu", db["Password"] ?? "devsu",
            builder.Configuration["RabbitMq:Username"] ?? "guest",
            builder.Configuration["RabbitMq:Password"] ?? "guest");
    }
}

// ============================================================
// 2. Cadena de conexion (host/puerto de config/env + credenciales de Vault)
// ============================================================
var dbSection = builder.Configuration.GetSection("Database");
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? dbSection["Host"] ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? dbSection["Port"] ?? "5432";
var dbName = dbSection["Name"] ?? "devsu_cuentas_net";
var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={secrets.DbUsername};Password={secrets.DbPassword}";

// ============================================================
// 3. DI por capa (hexagonal). Infrastructure registra el consumidor RabbitMQ.
// ============================================================
builder.Services.AddCuentasApplication();
builder.Services.AddCuentasInfrastructure(connectionString, rabbit =>
{
    builder.Configuration.GetSection(RabbitMqOptions.SectionName).Bind(rabbit);
    rabbit.Host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? rabbit.Host;
    if (!string.IsNullOrEmpty(secrets.RabbitUsername)) rabbit.Username = secrets.RabbitUsername;
    if (!string.IsNullOrEmpty(secrets.RabbitPassword)) rabbit.Password = secrets.RabbitPassword;
});

// ============================================================
// 4. Controllers + JSON (enums como texto) + validacion 400 -> ErrorResponse
// ============================================================
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var mensaje = string.Join(", ", context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors.Select(err => $"{e.Key}: {err.ErrorMessage}")));

        var body = new ErrorResponse(
            DateTimeOffset.UtcNow, StatusCodes.Status400BadRequest, "Bad Request",
            mensaje, context.HttpContext.Request.Path);

        return new ObjectResult(body) { StatusCode = StatusCodes.Status400BadRequest };
    };
});

// ============================================================
// 5. Manejo global de excepciones (404/409/422/500)
// ============================================================
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ============================================================
// 6. Seguridad: Resource Server JWT (Keycloak). Validacion por firma (JWKS).
// ============================================================
var keycloak = builder.Configuration.GetSection("Keycloak");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloak["Authority"];
        options.RequireHttpsMetadata = keycloak.GetValue("RequireHttpsMetadata", false);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async ctx =>
            {
                ctx.HandleResponse();
                await SecurityErrorResponder.WriteUnauthorizedAsync(ctx.HttpContext);
            },
            OnForbidden = ctx => SecurityErrorResponder.WriteForbiddenAsync(ctx.HttpContext)
        };
    });
builder.Services.AddAuthorization();

// ============================================================
// 7. Swagger / OpenAPI (con Bearer JWT)
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MS Cuentas API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT de Keycloak. Formato: Bearer {token}"
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
    });
});

var app = builder.Build();

// ============================================================
// 8. Migracion al arrancar (equivalente a Flyway). Flag MigrateOnStartup.
// ============================================================
if (builder.Configuration.GetValue("MigrateOnStartup", true))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CuentasDbContext>();
    db.Database.Migrate();
}

// ============================================================
// 9. Pipeline HTTP
// ============================================================
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "UP" })).AllowAnonymous();

app.MapControllers().RequireAuthorization();

app.Run();

// Necesario para WebApplicationFactory<Program> en las pruebas de integracion.
public partial class Program;
