using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using LMS.Data;
using QuestPDF.Infrastructure;
using System.Text.Json.Serialization;
using LMS.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;
using LMS.Models;
using LMS_SA_BACK.Services;

var builder = WebApplication.CreateBuilder(args);

// ================================
// QuestPDF License
// ================================
QuestPDF.Settings.License = LicenseType.Community;

// ================================
// Controllers
// ================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ================================
// Database
// ================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ================================
// JWT Authentication
// ================================
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = false,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/sessionhub"))
            {
                context.Token = accessToken;
            }
            else
            {
                var token = context.Request.Headers["Authorization"]
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(token) &&
                    token.StartsWith("Bearer "))
                {
                    context.Token = token.Substring("Bearer ".Length);
                }
            }

            return Task.CompletedTask;
        }
    };
});

// ================================
// Authorization
// ================================
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ================================
// Services
// ================================
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<SqlScriptExecutor>();
builder.Services.AddScoped<UserPaymentService>();

builder.Services.Configure<PhonePeSdkOptions>(
    builder.Configuration.GetSection("PhonePe"));

builder.Services.AddScoped<PhonePeService>();

builder.Services.AddHttpClient();

builder.Services.AddHttpClient<SmsService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHostedService<SmsBackgroundService>();

// ================================
// CORS
// ================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:3001",
                "https://skillascent.in",
                "https://www.skillascent.in",
                "https://lms.skillascent.in",
                "https://www.lms.skillascent.in",
                "https://mercury-uat.phonepe.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ================================
// SignalR
// ================================
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

// ================================
// Swagger
// ================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LMS API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ================================
// Build App
// ================================
var app = builder.Build();

// ================================
// Execute SQL Scripts
// ================================
using (var scope = app.Services.CreateScope())
{
    var executor = scope.ServiceProvider
        .GetRequiredService<SqlScriptExecutor>();

    await executor.ExecuteAllSqlFilesAsync();
}

// ================================
// Swagger
// ================================
app.UseSwagger();
app.UseSwaggerUI();

// ================================
// HTTPS
// ================================
app.UseHttpsRedirection();

// ================================
// Routing
// ================================
app.UseRouting();

// ================================
// CORS
// ================================
app.UseCors("AllowAll");

// ================================
// Authentication
// ================================
app.UseAuthentication();

// ================================
// Authorization
// ================================
app.UseAuthorization();

// ================================
// Static Files (Optional)
// ================================
app.UseStaticFiles();

// ================================
// Endpoints
// ================================
app.MapControllers();

app.MapHub<SessionHub>("/sessionhub");

// ================================
// Run
// ================================
app.Run();