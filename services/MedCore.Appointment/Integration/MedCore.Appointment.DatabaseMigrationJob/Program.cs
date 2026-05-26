using MedCore.Appointment.DatabaseMigrationJob;
using MedCore.DatabaseMigrationJob.Configuration;
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

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        //builder.Services.AddDbContext<>(opts =>
        //{
        //    opts.UseSqlServer(connectionString, sql =>
        //    {
        //        sql.MigrationsAssembly(migrationAssembly);
        //    });
        //});

        var host = builder.Build();
        var worker = host.Services.GetRequiredService<MigrationWorker>();

        return await worker.ExecuteAsync(CancellationToken.None);
    }
}