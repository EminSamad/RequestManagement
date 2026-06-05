using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RequestManagement.Infrastructure.Context;
using RequestManagement.Domain.Interfaces;
using RequestManagement.Infrastructure.Repositories.Implementations;
using RequestManagement.Application.Interfaces;
using RequestManagement.Application.Services;
using RequestManagement.API.Middlewares;
using RequestManagement.API.Services;
using Karambolo.Extensions.Logging.File;
using FluentValidation;
using FluentValidation.AspNetCore;
using RequestManagement.Application.Validators;
using Hangfire;
using Hangfire.PostgreSql;
using RequestManagement.Application.BackgroundJobs;
using RequestManagement.API.Filters;
using RequestManagement.API.Hubs;
using RequestManagement.Infrastructure.Seed;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.AddFile(options =>
{
    options.RootPath = AppContext.BaseDirectory;
    options.Files = new[]
    {
        new LogFileOptions { Path = "logs/log-<date>.txt" }
    };
});

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                    o.DocumentSettings = s =>
                    {
                        s.AddAuth("Bearer", new()
                        {
                            Name = "Authorization",
                            In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                            Type = NSwag.OpenApiSecuritySchemeType.Http,
                            Scheme = "Bearer",
                            BearerFormat = "JWT"
                        });
                    };
                });

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// Hangfire
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailJobService>();
builder.Services.AddScoped<ReminderJobService>();
builder.Services.AddScoped<ReportJobService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddHostedService<EmailConsumerService>();

builder.Services.AddSignalR();

// Cache
var redisConnection = builder.Configuration["Redis:ConnectionString"];

if (!string.IsNullOrEmpty(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

// JWT Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllConnectionsFilter() }
});

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();
app.MapHub<NotificationHub>("/hubs/notifications");

// Recurring jobs
RecurringJob.AddOrUpdate<ReportJobService>(
    "weekly-report",
    x => x.SendWeeklyReport(),
    "0 8 * * 6");

RecurringJob.AddOrUpdate<ReminderJobService>(
    "daily-reminder",
    x => x.SendReminder(),
    "0 9 * * *");

// DATABASE MIGRATION + SEEDER
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

app.Run();