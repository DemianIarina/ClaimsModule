using ClaimsModule.Application.Processors;
using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.Services;
using ClaimsModule.Infrastructure;
using ClaimsModule.Infrastructure.Config;
using ClaimsModule.Infrastructure.Processors;
using ClaimsModule.Infrastructure.Repositories;
using ClaimsModule.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Serilog;
using System;
using System.Text;

namespace ClaimsModule.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // stream to stdout (K8s native)
                .WriteTo.File("logs/claims-app-.log", rollingInterval: RollingInterval.Day) // local
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            var jwtConfig = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig["Issuer"],
                        ValidAudience = jwtConfig["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtConfig["Key"]!))
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
                options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
            });

            // Add services to the container.
            builder.Services.Configure<DecisionThresholds>(
                builder.Configuration.GetSection("DecisionThresholds"));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDecisionEngine, RuleBasedDecisionEngine>();
            QuestPDF.Settings.License = LicenseType.Community;
            builder.Services.AddScoped<IDocumentGenerator, AuthorizationDocumentGenerator>();
            builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
            builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddDbContext<ClaimsDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));
            builder.Services.AddScoped<IPolicyService, PolicyService>();
            builder.Services.AddScoped<IClaimService, ClaimService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "ClaimsModule API", Version = "v1" });

                // JWT Auth setup
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid JWT token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGci...\""
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ClaimsDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
