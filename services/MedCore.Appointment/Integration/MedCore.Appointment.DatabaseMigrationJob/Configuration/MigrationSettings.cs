namespace MedCore.Appointment.DatabaseMigrationJob.Configuration
{
    public class MigrationSettings
    {
        public required Dictionary<string, string> TargetMigration { get; set; }
    }
}