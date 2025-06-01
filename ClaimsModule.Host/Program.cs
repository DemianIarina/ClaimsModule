using ClaimsModule.Application.Repositories;
using ClaimsModule.Application.Services;
using ClaimsModule.Infrastructure;
using ClaimsModule.Infrastructure.Repositories;
using ClaimsModule.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

            // Add services to the container.
            builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
            builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
            builder.Services.AddScoped<IClaimService, ClaimService>();
            builder.Services.AddDbContext<ClaimsDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
