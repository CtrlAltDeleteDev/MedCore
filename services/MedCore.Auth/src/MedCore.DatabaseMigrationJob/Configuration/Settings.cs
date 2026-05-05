using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.DatabaseMigrationJob.Configuration
{
    public class Settings
    {
        public MigrationSettings MigrationSettings { get; set; }
        public bool SeedData {  get; set; }
    }
}
