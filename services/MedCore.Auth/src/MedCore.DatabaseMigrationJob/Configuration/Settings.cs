namespace MedCore.DatabaseMigrationJob.Configuration
{
    public class Settings
    {
        public required MigrationSettings MigrationSettings { get; set; }

        public bool SeedData { get; set; }
    }
}
