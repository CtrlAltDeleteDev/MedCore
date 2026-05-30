using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Serilog;
using System.Globalization;
using Serilog.Filters;

namespace MedCore.Appointment.Api
{
    public static class HostingExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!)),
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            services.AddAuthorization();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "MedCore Appointment API", Version = "v1" });

                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", document)] = []
                });
            });

            return services;
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            return app;
        }

        public static IHostApplicationBuilder ConfigureLogging(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSerilog(lc =>
            {
                lc.WriteTo.Logger(consoleLogger =>
                {
                    consoleLogger.WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                        formatProvider: CultureInfo.InvariantCulture);
                    if (builder.Environment.IsDevelopment())
                    {
                        consoleLogger.Filter.ByExcluding(Matching.FromSource("Duende.IdentityServer.Diagnostics.Summary"));
                    }
                });
                if (builder.Environment.IsDevelopment())
                {
                    lc.WriteTo.Logger(fileLogger =>
                    {
                        fileLogger
                            .WriteTo.File("./diagnostics/diagnostic.log", rollingInterval: RollingInterval.Day,
                                fileSizeLimitBytes: 1024 * 1024 * 10, // 10 MB
                                rollOnFileSizeLimit: true,
                                outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                                formatProvider: CultureInfo.InvariantCulture)
                            .Filter
                            .ByIncludingOnly(Matching.FromSource("Duende.IdentityServer.Diagnostics.Summary"));
                    }).Enrich.FromLogContext().ReadFrom.Configuration(builder.Configuration);
                }
            });
            return builder;
        }

    }
}
