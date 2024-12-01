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

// ��������� �����������
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ��������� ���� ������
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� �������������� � JWT
builder.Services.AddAuthentication(options =>
{
    // ������������� ����� �������������� �� ��������� ��� ��������� JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // ��� JWT
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

    // ��������� ������� ��� �����������
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
    options.LoginPath = "/api/Auth/login"; // ������� ���������� ���� ��� �����
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401; // ���������� 401 ������ ���������
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403; // ���������� 403 ������ ���������
            return Task.CompletedTask;
        },

    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["OAuth:Google:ClientId"];
    options.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // ���������� ���� ��� �����
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


// ���������� ��������� ������������
builder.Services.AddControllersWithViews();

// ��������� Quartz ��� ������������ �����
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

// ����������� �������� ��� ���������
builder.Services.AddScoped<ILogWriter, LogWriter>(provider =>
{
    var logFilePath = "access.log"; // ���� � ����� �����
    return new LogWriter(logFilePath);
});
builder.Services.AddScoped<ILogProcessor, LogProcessor>();
builder.Services.AddScoped<LogProcessingJob>();

// ���������� Swagger ��� ������������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAll");
// ���������� ���������� ����������

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
                Console.WriteLine("����� �� �������� ��� ����� ������������ ������.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������ ��� ��������� ������: {ex.Message}");
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

        // ����� ����� �������� �������������� ������, ���� �����
    }
    else
    {
        Console.WriteLine("No Authorization header received.");
    }

    await next(); // �������� ������� ������ � pipeline
});




// Middleware ��� ����������� ��������
app.Use(async (context, next) =>
{
    var ipAddress = context.Connection.RemoteIpAddress;

    // �������������� IPv6 � IPv4
    if (ipAddress != null && ipAddress.IsIPv4MappedToIPv6)
    {
        ipAddress = ipAddress.MapToIPv4();
    }

    Console.WriteLine($"Incoming Request from IP: {ipAddress}");
    await next();
});



// Middleware ��� ������ �����
app.Use(async (context, next) =>
{
    var logWriter = context.RequestServices.GetRequiredService<ILogWriter>();
    await logWriter.WriteLogAsync(
        context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
        DateTime.UtcNow,
        $"{context.Request.Method} {context.Request.Path}",
        context.Response.StatusCode,
        0 // ������ ������ (�� ��������� 0, ���� �� ��������)
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

// ������������ ��� ������ ����������
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ��������� ����������� ������
app.UseStaticFiles();

// ��������� ���������
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// �������� ��� ������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
