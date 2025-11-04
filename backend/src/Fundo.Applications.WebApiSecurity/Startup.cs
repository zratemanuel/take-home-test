using Fundo.Applications.Domain.Interfaces;
using Fundo.Applications.Domain.Services;
using Fundo.Applications.Repository;
using Fundo.Applications.Repository.Interface;
using Fundo.Applications.Repository.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Fundo.Applications.WebApiSecurity;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //Run Container based connection 
        //var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        //var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        //var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        //var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword}";

        //Run Local based connection
        var connectionString = Configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ContextDB>(options =>
            options.UseSqlServer(connectionString!,
                                 b => b.MigrationsAssembly("Fundo.Applications.WebApi")));

        services.AddControllers();
        services.AddSwaggerGen();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IApplicantRepository, ApplicantRepository>();

        string key = Configuration.GetSection("jwt:secretKey").Value!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "yourdomain.com",
                ValidAudience = "yourdomain.com",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });

        services.AddAuthorization();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Security API", Version = "v1" });

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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Security API v1");
            });
        }

        app.UseCors(builder =>
            builder
              .AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());

        
    }
}
