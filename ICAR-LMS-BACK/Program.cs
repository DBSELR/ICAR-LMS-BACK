////using Microsoft.AspNetCore.Authentication.JwtBearer;
////using Microsoft.IdentityModel.Tokens;
////using Microsoft.EntityFrameworkCore;
////using System.Text;
////using LMS.Data;
////using QuestPDF.Infrastructure;
////using System.Text.Json.Serialization;
////using LMS.Services;

////var builder = WebApplication.CreateBuilder(args);

////// ✅ Enable community license for QuestPDF
////QuestPDF.Settings.License = LicenseType.Community;

////// ✅ Register controllers with cycle-safe JSON serialization
////builder.Services.AddControllers().AddJsonOptions(options =>
////{
////    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
////    options.JsonSerializerOptions.WriteIndented = true;
////});

////// ✅ Register the DbContext using SQL Server
////builder.Services.AddDbContext<AppDbContext>(options =>
////    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////// ✅ Configure JWT Authentication
////builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
////    .AddJwtBearer(options =>
////    {
////        options.RequireHttpsMetadata = false;
////        options.TokenValidationParameters = new TokenValidationParameters
////        {
////            ValidateIssuer = false,
////            ValidateAudience = false,
////            ValidateLifetime = true,
////            ValidateIssuerSigningKey = true,
////            IssuerSigningKey = new SymmetricSecurityKey(
////                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
////        };
////    });
////builder.Services.AddScoped<IFeeService, FeeService>();


////// ✅ Authorization policies for role-based access
////builder.Services.AddAuthorization(options =>
////{
////    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
////    options.AddPolicy("InstructorOnly", policy => policy.RequireRole("Instructor"));
////    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
////});

////// ✅ Enable CORS for React app
////builder.Services.AddCors(options =>
////{
////    options.AddPolicy("AllowReactApp", policy =>
////    {
////        policy.WithOrigins("http://localhost:3000", "https://lms.andhrauniversity-sde.com")
////              .AllowAnyHeader()
////              .AllowAnyMethod();
////    });
////});
////builder.Services.AddTransient<SqlScriptExecutor>();






////var app = builder.Build();
////using (var scope = app.Services.CreateScope())
////{
////    var executor = scope.ServiceProvider.GetRequiredService<SqlScriptExecutor>();
////    await executor.ExecuteAllSqlFilesAsync();
////}

////// ✅ Middleware Pipeline
////app.UseRouting();
////app.UseCors("AllowReactApp");
////app.UseAuthentication();
////app.UseAuthorization();

////app.MapControllers();
////app.UseStaticFiles(); // REQUIRED to serve wwwroot/*


////// ✅ Log route hits
////app.Use(async (context, next) =>
////{
////    Console.WriteLine($"➡️ Route hit: {context.Request.Method} {context.Request.Path}");
////    await next();
////});

////app.Run();





//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.EntityFrameworkCore;
//using System.Text;
//using LMS.Data;
//using QuestPDF.Infrastructure;
//using System.Text.Json.Serialization;
//using LMS.Services;
//using Microsoft.Extensions.FileProviders;
//using Microsoft.OpenApi.Models;
//using Microsoft.AspNetCore.SignalR;

//var builder = WebApplication.CreateBuilder(args);


//// ✅ Enable community license for QuestPDF
//QuestPDF.Settings.License = LicenseType.Community;

//// ✅ Register controllers with cycle-safe JSON serialization
//builder.Services.AddControllers().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//    options.JsonSerializerOptions.WriteIndented = true;
//});

////builder.Services.AddEndpointsApiExplorer();

////builder.Services.AddSwaggerGen(c =>
////{
////    c.SwaggerDoc("v1", new OpenApiInfo
////    {
////        Title = "LMS API",
////        Version = "v1"
////    });

////    // ✅ Add JWT bearer support
////    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
////    {
////        In = ParameterLocation.Header,
////        Description = "Enter JWT token with **Bearer** prefix. Example: Bearer eyJhbGciOiJIUzI1...",
////        Name = "Authorization",
////        Type = SecuritySchemeType.Http,
////        Scheme = "bearer",
////        BearerFormat = "JWT"
////    });

////    c.AddSecurityRequirement(new OpenApiSecurityRequirement
////    {
////        {
////            new OpenApiSecurityScheme
////            {
////                Reference = new OpenApiReference
////                {
////                    Type = ReferenceType.SecurityScheme,
////                    Id = "Bearer"
////                }
////            },
////            Array.Empty<string>()
////        }
////    });
////});


//// ✅ Register the DbContext using SQL Server
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// ✅ Configure JWT Authentication
////builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
////    .AddJwtBearer(options =>
////    {
////        options.RequireHttpsMetadata = false;
////        options.SaveToken = true;

////        options.TokenValidationParameters = new TokenValidationParameters
////        {
////            ValidateIssuer = false,
////            ValidateAudience = false,
////            ValidateLifetime = true,
////            ValidateIssuerSigningKey = true,
////            IssuerSigningKey = new SymmetricSecurityKey(
////                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
////        };

////        // ✅ Read token from Authorization header or cookie
////        options.Events = new JwtBearerEvents
////        {
////            OnMessageReceived = context =>
////            {
////                // Check Authorization header
////                var token = context.Request.Headers["Authorization"].FirstOrDefault();
////                if (!string.IsNullOrWhiteSpace(token) && token.StartsWith("Bearer "))
////                {
////                    context.Token = token.Substring("Bearer ".Length);
////                }
////                // Check cookie fallback
////                else if (context.Request.Cookies.ContainsKey("X-Access-Token"))
////                {
////                    context.Token = context.Request.Cookies["X-Access-Token"];
////                }

////                return Task.CompletedTask;
////            }
////        };
////    });

//// JWT Authentication
//var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.RequireHttpsMetadata = false;
//        options.SaveToken = true;
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.FromMinutes(1),
//            ValidateIssuerSigningKey = false,
//            IssuerSigningKey = new SymmetricSecurityKey(key)
//        };
//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                // For SignalR WebSocket
//                var accessToken = context.Request.Query["access_token"];
//                var path = context.HttpContext.Request.Path;
//                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/sessionhub"))
//                {
//                    context.Token = accessToken;
//                }
//                else
//                {
//                    // Normal API auth
//                    var token = context.Request.Headers["Authorization"].FirstOrDefault();
//                    if (!string.IsNullOrWhiteSpace(token) && token.StartsWith("Bearer "))
//                    {
//                        context.Token = token.Substring("Bearer ".Length);
//                    }
//                }
//                return Task.CompletedTask;
//            }
//        };
//    });

//// ✅ Global authorization
//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//});

//builder.Services.AddAuthorization();

//builder.Services.AddScoped<IFeeService, FeeService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddTransient<SqlScriptExecutor>();

//builder.Services.AddHttpClient();


//// ✅ Authorization policies for role-based access
////builder.Services.AddAuthorization(options =>
////{
////    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
////    options.AddPolicy("InstructorOnly", policy => policy.RequireRole("Instructor"));
////    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
////});

//// ✅ Enable CORS for React app
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "https://edvedha.dbasesolutions.in", "https://www.edvedha.dbasesolutions.in")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials(); //  CRITICAL for cookie/session use
//    });
//});

//builder.Services.AddSignalR();
//builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

//var app = builder.Build();

////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////}

//// ✅ Execute SQL scripts (if any)
//using (var scope = app.Services.CreateScope())
//{
//    var executor = scope.ServiceProvider.GetRequiredService<SqlScriptExecutor>();
//    await executor.ExecuteAllSqlFilesAsync();
//}

//app.UseRouting();                   // 2. Enable endpoint routing



//// ✅ Handle CORS preflight requests for static files
//app.Use(async (context, next) =>
//{
//    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
//    if (!string.IsNullOrEmpty(authHeader))
//    {
//        Console.WriteLine("Authorization Header: " + authHeader);
//    }

//    if (context.Request.Method == "OPTIONS" &&
//        context.Request.Path.StartsWithSegments("/uploads"))
//    {
//        context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
//        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
//        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
//        context.Response.StatusCode = 200;
//        await context.Response.CompleteAsync();
//        return;
//    }

//    await next();
//});

//app.UseCors("AllowAll");       // 3. CORS must be before auth
//app.UseStaticFiles();
//app.UseAuthentication();            // 4. Auth middleware
//app.UseAuthorization();             // 5. Authorization

//// default wwwroot
//// ✅ Static files from "uploads" with CORS headers
////app.UseStaticFiles(new StaticFileOptions
////{
////    FileProvider = new PhysicalFileProvider(
////        Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
////    RequestPath = "/uploads",
////    OnPrepareResponse = ctx =>
////    {
////        // CORS headers for static files
////        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
////        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
////        ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
////    }
////});


//// ✅ Map controllers
//app.MapControllers();

//app.MapHub<SessionHub>("/sessionhub");

//app.Run();


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

var builder = WebApplication.CreateBuilder(args);


// ✅ Enable community license for QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// ✅ Register controllers with cycle-safe JSON serialization
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "LMS API",
//        Version = "v1"
//    });

//    // ✅ Add JWT bearer support
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Enter JWT token with **Bearer** prefix. Example: Bearer eyJhbGciOiJIUzI1...",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});


// ✅ Register the DbContext using SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Configure JWT Authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.RequireHttpsMetadata = false;
//        options.SaveToken = true;

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(
//                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };

//        // ✅ Read token from Authorization header or cookie
//        options.Events = new JwtBearerEvents
//        {
//            OnMessageReceived = context =>
//            {
//                // Check Authorization header
//                var token = context.Request.Headers["Authorization"].FirstOrDefault();
//                if (!string.IsNullOrWhiteSpace(token) && token.StartsWith("Bearer "))
//                {
//                    context.Token = token.Substring("Bearer ".Length);
//                }
//                // Check cookie fallback
//                else if (context.Request.Cookies.ContainsKey("X-Access-Token"))
//                {
//                    context.Token = context.Request.Cookies["X-Access-Token"];
//                }

//                return Task.CompletedTask;
//            }
//        };
//    });

// JWT Authentication
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
                // For SignalR WebSocket
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/sessionhub"))
                {
                    context.Token = accessToken;
                }
                else
                {
                    // Normal API auth
                    var token = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(token) && token.StartsWith("Bearer "))
                    {
                        context.Token = token.Substring("Bearer ".Length);
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

// ✅ Global authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<SqlScriptExecutor>();

builder.Services.AddHttpClient();


// ✅ Authorization policies for role-based access
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("InstructorOnly", policy => policy.RequireRole("Instructor"));
//    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
//});



// ✅ Enable CORS for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "https://edvedha.dbasesolutions.in", "https://www.edvedha.dbasesolutions.in")
               .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); //  CRITICAL for cookie/session use
    });
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

// ✅ Execute SQL scripts (if any)
using (var scope = app.Services.CreateScope())
{
    var executor = scope.ServiceProvider.GetRequiredService<SqlScriptExecutor>();
    await executor.ExecuteAllSqlFilesAsync();
}

app.UseRouting();                   // 2. Enable endpoint routing



// ✅ Handle CORS preflight requests for static files
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    if (!string.IsNullOrEmpty(authHeader))
    {
        Console.WriteLine("Authorization Header: " + authHeader);
    }

    if (context.Request.Method == "OPTIONS" &&
        context.Request.Path.StartsWithSegments("/uploads"))
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }

    await next();
});

app.UseCors("AllowAll");       // 3. CORS must be before auth
app.UseStaticFiles();
app.UseAuthentication();            // 4. Auth middleware
app.UseAuthorization();             // 5. Authorization

// default wwwroot
// ✅ Static files from "uploads" with CORS headers
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
//    RequestPath = "/uploads",
//    OnPrepareResponse = ctx =>
//    {
//        // CORS headers for static files
//        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:3000");
//        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization");
//        ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
//    }
//});


// ✅ Map controllers
app.MapControllers();

app.MapHub<SessionHub>("/sessionhub");

app.Run();



