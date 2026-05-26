using MedCore.DatabaseMigrationJob.Configuration;
using Serilog;

namespace MedCore.Appointment.DatabaseMigrationJob
{
    public class MigrationWorker
    {
        private readonly Settings _settings;

        public MigrationWorker(Settings settings)
        {
            _settings = settings;
        }

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Starting database migration proccess...");
                //await _applicationDbContext.Database
                //    .MigrateAsync(_settings.MigrationSettings.TargetMigration[nameof(ApplicationDbContext)], cancellationToken);
                Log.Information("Done migrating database");

                if (_settings.SeedData)
                {
                    Log.Information("Seeding database...");
                    //await EnsureSeedData(_applicationDbContext, _userMgr, cancellationToken);
                    Log.Information("Done seeding database. Exiting.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception while migrate databases");
                return 0;
            }

            return 1;
        }

        private async Task EnsureSeedData(CancellationToken cancellationToken)
        {
        }

    }
}
