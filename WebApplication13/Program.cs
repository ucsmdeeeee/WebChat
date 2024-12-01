using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Security.Claims;
using System.Text;
using WebApplication13.Jobs;
using WebApplication13.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;

var builder = WebApplication.CreateBuilder(args);

// Настройка логирования
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Настройка базы данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка аутентификации с JWT
builder.Services.AddAuthentication(options =>
{
    // Устанавливаем схему аутентификации по умолчанию для обработки JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Для JWT
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    // Включение событий для логирования
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("---- JWT Authentication Failed ----");
            Console.WriteLine($"Exception: {context.Exception.GetType().Name} - {context.Exception.Message}");
            Console.WriteLine($"Stack Trace: {context.Exception.StackTrace}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("---- JWT Token Successfully Validated ----");
            Console.WriteLine($"Principal: {context.Principal?.Identity?.Name}");
            Console.WriteLine("Claims:");
            foreach (var claim in context.Principal.Claims)
            {
                Console.WriteLine($" - {claim.Type}: {claim.Value}");
            }
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            Console.WriteLine("---- JWT Message Received ----");
            Console.WriteLine($"Token: {context.Token}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine("---- JWT Authentication Challenge ----");
            Console.WriteLine($"Error: {context.Error}");
            Console.WriteLine($"Error Description: {context.ErrorDescription}");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            Console.WriteLine("---- JWT Forbidden ----");
            Console.WriteLine("The user is authenticated but does not have access to this resource.");
            return Task.CompletedTask;
        }

    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/api/Auth/login"; // Укажите правильный путь для входа
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401; // Возвращаем 401 вместо редиректа
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403; // Возвращаем 403 вместо редиректа
            return Task.CompletedTask;
        },

    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["OAuth:Google:ClientId"];
    options.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Используем куки для входа
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


// Добавление поддержки контроллеров
builder.Services.AddControllersWithViews();

// Настройка Quartz для планирования задач
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.AddJob<LogProcessingJob>(opts => opts.WithIdentity("LogProcessingJob"));
    q.AddTrigger(opts => opts
        .ForJob("LogProcessingJob")
        .WithIdentity("LogProcessingJobTrigger")
        .WithSimpleSchedule(schedule => schedule
            .WithIntervalInMinutes(10)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService();

// Регистрация сервисов для внедрения
builder.Services.AddScoped<ILogWriter, LogWriter>(provider =>
{
    var logFilePath = "access.log"; // Путь к файлу логов
    return new LogWriter(logFilePath);
});
builder.Services.AddScoped<ILogProcessor, LogProcessor>();
builder.Services.AddScoped<LogProcessingJob>();

// Добавление Swagger для документации
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAll");
// Глобальный обработчик исключений

app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        Console.WriteLine("---- JWT Token Decoding ----");

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);

                Console.WriteLine("Header:");
                foreach (var item in jwtToken.Header)
                {
                    Console.WriteLine($"  {item.Key}: {item.Value}");
                }

                Console.WriteLine("\nPayload:");
                foreach (var claim in jwtToken.Claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }
            }
            else
            {
                Console.WriteLine("Токен не читается или имеет неправильный формат.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке токена: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("No Authorization header received.");
    }

    await next();
});


app.UseAuthorization();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != null)
        {
            Console.WriteLine($"Global Exception: {exceptionHandlerPathFeature.Error}");
        }
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred.");
    });
});
app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        Console.WriteLine("Authorization Header Found:");
        Console.WriteLine(token);

        // Здесь можно добавить дополнительную логику, если нужно
    }
    else
    {
        Console.WriteLine("No Authorization header received.");
    }

    await next(); // Передача запроса дальше в pipeline
});




// Middleware для логирования запросов
app.Use(async (context, next) =>
{
    var ipAddress = context.Connection.RemoteIpAddress;

    // Преобразование IPv6 в IPv4
    if (ipAddress != null && ipAddress.IsIPv4MappedToIPv6)
    {
        ipAddress = ipAddress.MapToIPv4();
    }

    Console.WriteLine($"Incoming Request from IP: {ipAddress}");
    await next();
});



// Middleware для записи логов
app.Use(async (context, next) =>
{
    var logWriter = context.RequestServices.GetRequiredService<ILogWriter>();
    await logWriter.WriteLogAsync(
        context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
        DateTime.UtcNow,
        $"{context.Request.Method} {context.Request.Path}",
        context.Response.StatusCode,
        0 // Размер ответа (по умолчанию 0, если не известен)
    );
    await next();
});
app.Use(async (context, next) =>
{
    Console.WriteLine("Before Authorization Middleware:");
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        Console.WriteLine($"Authorization Header: {context.Request.Headers["Authorization"]}");
    }
    await next();
    Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
});

// Конфигурация для режима разработки
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Поддержка статических файлов
app.UseStaticFiles();

// Настройка маршрутов
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Маршруты для контроллеров
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
