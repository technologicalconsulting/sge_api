using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using sge_api.Data;
using sge_api.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurar controladores
builder.Services.AddControllers();

// 🔹 Configurar conexión a PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Configurar autenticación con JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? "ClavePorDefecto");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();

// 🔹 Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SGE API",
        Version = "v1",
        Description = "API para autenticación y gestión de usuarios en el sistema SGE",
        Contact = new OpenApiContact
        {
            Name = "Soporte SGE",
            Email = "soporte@sge.com",
            Url = new Uri("https://sge.com")
        }
    });
});

var app = builder.Build();

// 🔹 Habilitar Swagger al iniciar el proyecto
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SGE API v1");
    c.RoutePrefix = string.Empty; // Acceso directo a Swagger al iniciar el proyecto
});

// 🔹 Habilitar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Habilitar los controladores
app.MapControllers();

// 🔹 Iniciar la API
app.Run();
