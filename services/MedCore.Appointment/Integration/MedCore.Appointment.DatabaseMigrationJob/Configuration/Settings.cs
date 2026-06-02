using MedCore.Appointment.DatabaseMigrationJob.Configuration;

namespace MedCore.DatabaseMigrationJob.Configuration
{
    public class Settings
    {
        public MigrationSettings MigrationSettings { get; set; }
        public bool SeedData {  get; set; }
    }
}
