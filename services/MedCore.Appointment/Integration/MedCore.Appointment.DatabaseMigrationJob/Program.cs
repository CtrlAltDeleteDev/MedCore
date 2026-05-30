using MedCore.Appointment.Api;
using MedCore.Appointment.DatabaseMigrationJob;
using MedCore.Appoitment.Data;
using MedCore.DatabaseMigrationJob.Configuration;
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
        builder.Services.AddSingleton<Settings>(settings);

        builder.Logging.ClearProviders();
        builder.ConfigureLogging();

        builder.Services.AddSingleton<MigrationWorker>();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        var migrationAssembly = typeof(AppoitmentDbContext).Assembly.FullName;

        builder.Services.AddDbContext<AppoitmentDbContext>(opts =>
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