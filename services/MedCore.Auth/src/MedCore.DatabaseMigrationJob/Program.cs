using MedCore.Auth.IdentityServer;
using MedCore.Auth.IdentityServer.Data;
using MedCore.Auth.IdentityServer.Models;
using MedCore.DatabaseMigrationJob;
using MedCore.DatabaseMigrationJob.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
        var settings = builder.Configuration.Get<Settings>();
        if(settings is null )
        {
            throw new InvalidOperationException("Settings configuration is missing.");
        }
        builder.Services.AddSingleton<Settings>(settings);

        builder.Logging.ClearProviders();
        HostingExtensions.ConfigureLogging(builder);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddSingleton<MigrationWorker>();
        builder.Services.AddSingleton(new DbContextOptions<DbContext>());

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        var migrationAssembly = typeof(MedCore.Auth.IdentityServer.HostingExtensions).Assembly.FullName;

        builder.Services.AddDbContext<ApplicationDbContext>(opts =>
        {
            opts.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(migrationAssembly);
            });
        });

        var host = builder.Build();
        var worker = host.Services.GetRequiredService<MigrationWorker>();

        return await worker.ExecuteAsync(CancellationToken.None);
    }
}