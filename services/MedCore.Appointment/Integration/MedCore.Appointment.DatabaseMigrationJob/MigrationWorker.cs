using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MedCore.DatabaseMigrationJob.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MedCore.Appointment.DatabaseMigrationJob
{
    public class MigrationWorker
    {
        private readonly Settings _settings;
        private readonly AppoitmentDbContext _dbContext;

        public MigrationWorker(Settings settings, AppoitmentDbContext dbContext)
        {
            _settings = settings;
            _dbContext = dbContext;
        }

        public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Starting database migration proccess...");
                await _dbContext.Database
                    .MigrateAsync(_settings.MigrationSettings.TargetMigration[nameof(AppoitmentDbContext)], cancellationToken);
                Log.Information("Done migrating database");

                if (_settings.SeedData)
                {
                    Log.Information("Seeding database...");
                    await EnsureSeedData(_dbContext, cancellationToken);
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

        private async Task EnsureSeedData(AppoitmentDbContext dbContext, CancellationToken cancellationToken)
        {
            if (await dbContext.Set<Employee>().AnyAsync(cancellationToken))
            {
                Log.Information("Database already seeded. Skipping.");
                return;
            }

            var baseDate = DateTime.UtcNow.Date;

            var skillCaries = new Skill { Name = "Лікування карієсу", Description = "Пломбування та відновлення зубів при карієсі" };
            var skillExtraction = new Skill { Name = "Видалення зубів", Description = "Проста та хірургічна екстракція зубів" };
            var skillWhitening = new Skill { Name = "Відбілювання зубів", Description = "Професійне відбілювання системою Beyond" };
            var skillOrtho = new Skill { Name = "Ортодонтія", Description = "Брекети, елайнери, ретейнери" };
            var skillImplants = new Skill { Name = "Дентальні імпланти", Description = "Встановлення та відновлення дентальних імплантів" };
            var skillCleaning = new Skill { Name = "Ультразвукове чищення", Description = "Видалення зубного каменю та нальоту" };
            var skillPerio = new Skill { Name = "Лікування пародонту", Description = "Лікування захворювань ясен та пародонту" };
            var skillProsthetics = new Skill { Name = "Протезування", Description = "Коронки, вініри, мости, знімні протези" };

            var employees = new List<Employee>
            {
                new Employee
                {
                    FullName = "Мороз Олексій Іванович",
                    Title = "Лікар-стоматолог терапевт",
                    IsActive = true,
                    Skills = new List<Skill> { skillCaries, skillWhitening, skillCleaning },
                    Meets = new List<Meet>
                    {
                        new Meet { Subject = "Лікування карієсу — верхній лівий моляр", PatientId = 1,
                            StartTime = baseDate.AddDays(1).AddHours(9),   EndTime = baseDate.AddDays(1).AddHours(10) },
                        new Meet { Subject = "Профілактичний огляд", PatientId = 2,
                            StartTime = baseDate.AddDays(1).AddHours(11),  EndTime = baseDate.AddDays(1).AddHours(11).AddMinutes(30) },
                        new Meet { Subject = "Відбілювання зубів", PatientId = 3,
                            StartTime = baseDate.AddDays(3).AddHours(14),  EndTime = baseDate.AddDays(3).AddHours(15).AddMinutes(30) }
                    }
                },
                new Employee
                {
                    FullName = "Ковальчук Наталія Сергіївна",
                    Title = "Лікар-ортодонт",
                    IsActive = true,
                    Skills = new List<Skill> { skillOrtho, skillCaries },
                    Meets = new List<Meet>
                    {
                        new Meet { Subject = "Первинна консультація ортодонта", PatientId = 4,
                            StartTime = baseDate.AddDays(2).AddHours(10),  EndTime = baseDate.AddDays(2).AddHours(11) },
                        new Meet { Subject = "Встановлення брекетів", PatientId = 5,
                            StartTime = baseDate.AddDays(4).AddHours(9),   EndTime = baseDate.AddDays(4).AddHours(11) }
                    }
                },
                new Employee
                {
                    FullName = "Шевченко Василь Петрович",
                    Title = "Лікар-хірург стоматолог",
                    IsActive = true,
                    Skills = new List<Skill> { skillExtraction, skillImplants },
                    Meets = new List<Meet>
                    {
                        new Meet { Subject = "Видалення зуба мудрості", PatientId = 6,
                            StartTime = baseDate.AddDays(1).AddHours(13),  EndTime = baseDate.AddDays(1).AddHours(14) },
                        new Meet { Subject = "Встановлення імпланту (позиція 36)", PatientId = 7,
                            StartTime = baseDate.AddDays(5).AddHours(10),  EndTime = baseDate.AddDays(5).AddHours(12) }
                    }
                },
                new Employee
                {
                    FullName = "Лисенко Ірина Олексіївна",
                    Title = "Лікар-пародонтолог",
                    IsActive = true,
                    Skills = new List<Skill> { skillPerio, skillCleaning, skillProsthetics },
                    Meets = new List<Meet>
                    {
                        new Meet { Subject = "Ультразвукове чищення зубів", PatientId = 8,
                            StartTime = baseDate.AddDays(2).AddHours(12),  EndTime = baseDate.AddDays(2).AddHours(13) },
                        new Meet { Subject = "Лікування гінгівіту", PatientId = 9,
                            StartTime = baseDate.AddDays(6).AddHours(9),   EndTime = baseDate.AddDays(6).AddHours(10) }
                    }
                }
            };

            dbContext.AddRange(employees);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
