using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;
using TaskManager.API.Data;
using TaskManager.API.Services;
using TaskManager.API.Models;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDevClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")   // URL del front en desarrollo
            .AllowAnyHeader()                        // Permitir cualquier header (por ejemplo Authorization)
            .AllowAnyMethod()                        // Permitir GET, POST, PUT, DELETE, etc.
            .AllowCredentials();                     // Si usas cookies o autenticación basada en credenciales
    });
});

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// Configure Authorization Policies
builder.Services.AddAuthorizationBuilder()
                                       // Configure Authorization Policies
                                       .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
                                       // Configure Authorization Policies
                                       .AddPolicy("SupervisorOrAdmin", policy => policy.RequireRole("Admin", "Supervisor"))
                                       // Configure Authorization Policies
                                       .AddPolicy("EmployeeOrAbove", policy => policy.RequireRole("Admin", "Supervisor", "Employee"));

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    // Configuración para JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT en el campo: Bearer {tu token}"
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
// Configurar la base de datos y seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var hasher = services.GetRequiredService<IPasswordHasher<User>>();

        // Aplicar migraciones
        context.Database.Migrate();

        // Crear usuario admin si no existe
        if (!context.Users.Any(u => u.Role == Role.Admin))
        {
            var admin = new User
            {
                Name = "Admin",
                Email = "admin@example.com",
                Role = Role.Admin
            };
            admin.PasswordHash = hasher.HashPassword(admin, "AdminPassword123!");
            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error durante la inicialización de la base de datos");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReactDevClient");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();