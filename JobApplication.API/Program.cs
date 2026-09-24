using Hangfire;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Interfaces.Auth;
using JobApplication.Application.Services;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Auth;
using JobApplication.Infrastructure.Persistence;
using JobApplication.Infrastructure.Repositories;
using JobApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

namespace JobApplication.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Controllers with JSON String Enum support
            builder.Services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Database Context
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Authentication & JWT Settings
            var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
            var jwtKey = !string.IsNullOrWhiteSpace(jwt?.Key)
                ? jwt.Key
                : "development-secret-key-that-is-at-least-32-characters-long!";
            var jwtIssuer = jwt?.Issuer ?? "JobApplication.API";
            var jwtAudience = jwt?.Audience ?? "JobApplication.Client";

            builder.Services.Configure<JwtSettings>(options =>
            {
                options.Key = jwtKey;
                options.Issuer = jwtIssuer;
                options.Audience = jwtAudience;
                options.ExpiryMinutes = jwt?.ExpiryMinutes > 0 ? jwt.ExpiryMinutes : 60;
            });

            // Identity & Auth Services
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            // Repositories & Domain Services
            builder.Services.AddScoped<IJobRepository, JobRepository>();
            builder.Services.AddScoped<IJobService, JobService>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();

            // Hangfire Background Services
            builder.Services.AddScoped<IBackgroundJobScheduler, HangfireBackgroundJobScheduler>();
            builder.Services.AddScoped<INotificationService, EmailNotificationService>();
            builder.Services.AddScoped<IJobCleanupService, JobCleanupService>();

            // MediatR (CQRS)
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(JobApplication.Application.AssemblyReference).Assembly));

            // Hangfire Storage & Server
            var hangfireConnection = builder.Configuration.GetConnectionString("HangfireConnection")
                ?? connectionString;

            builder.Services.AddHangfire(config => config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnection));

            builder.Services.AddHangfireServer(options =>
            {
                options.WorkerCount = Math.Min(Environment.ProcessorCount * 5, 20);
            });

            // OpenAPI & Scalar Documentation
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure HTTP Request Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Job Application API");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            // Hangfire Dashboard with open local/demo authorization
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "Job Application API - Hangfire Dashboard",
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });

            // Register Hangfire Recurring Jobs:
            // Auto-close jobs open for more than 30 days
            // Cron expression: "0 0 * * *" (Daily at midnight UTC)
            RecurringJob.AddOrUpdate<IJobCleanupService>(
                "auto-close-expired-jobs",
                service => service.CloseExpiredJobsAsync(CancellationToken.None),
                Cron.Daily());

            app.MapControllers();

            app.Run();
        }
    }
}
