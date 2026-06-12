using MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery;
using MedCore.Appointment.Application.Tests.Helpers;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Infrastructure.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace MedCore.Appointment.Application.Tests.Handlers
{
    public class GetActiveDoctorsQueryHandlerTests
    {
        [Fact]
        public async Task Returns_all_active_doctors()
        {
            const string db = "get_active_all";
            await SeedAsync(db, includeInactive: false);

            var result = await BuildHandler(db).Handle(
                new GetActiveDoctorsQuery(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Length);
        }

        [Fact]
        public async Task Returns_only_active_doctors_excludes_inactive()
        {
            const string db = "get_active_excludes_inactive";
            await SeedAsync(db, includeInactive: true);

            var result = await BuildHandler(db).Handle(
                new GetActiveDoctorsQuery(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Length);
            Assert.All(result.Value!, d => Assert.DoesNotContain("Inactive", d.FullName));
        }

        [Fact]
        public async Task Returns_empty_when_no_active_doctors()
        {
            const string db = "get_active_empty";
            await using var db2 = DbContextFactory.Create(db);

            var inactiveDoctor = new Employee { Id = 1, FullName = "Dr. Inactive", Title = "None", IsActive = false };
            db2.Add(inactiveDoctor);
            await db2.SaveChangesAsync();

            var result = await BuildHandler(db).Handle(
                new GetActiveDoctorsQuery(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task Returned_dto_includes_skill_ids()
        {
            const string db = "get_active_skills_mapped";
            await SeedAsync(db, includeInactive: false);

            var result = await BuildHandler(db).Handle(
                new GetActiveDoctorsQuery(), CancellationToken.None);

            Assert.True(result.IsSuccess);
            var doctorWithBothSkills = result.Value!.Single(d => d.Id == 1);
            Assert.Equal(2, doctorWithBothSkills.SkillIds.Length);
        }

        private static GetActiveDoctorsQueryHandler BuildHandler(string dbName)
        {
            var db = DbContextFactory.Create(dbName);
            return new(new EmployeeRepository(db), NullLogger<GetActiveDoctorsQueryHandler>.Instance);
        }

        private static async Task SeedAsync(string dbName, bool includeInactive)
        {
            await using var db = DbContextFactory.Create(dbName);

            var skillCaries = new Skill { Id = 1, Name = "Caries Treatment", Description = string.Empty };
            var skillCleaning = new Skill { Id = 2, Name = "Cleaning", Description = string.Empty };

            var doctor1 = new Employee { Id = 1, FullName = "Dr. A", Title = "Therapist", IsActive = true };
            doctor1.Skills.Add(skillCaries);
            doctor1.Skills.Add(skillCleaning);

            var doctor2 = new Employee { Id = 2, FullName = "Dr. B", Title = "Surgeon", IsActive = true };
            doctor2.Skills.Add(skillCaries);

            db.AddRange(doctor1, doctor2);

            if (includeInactive)
            {
                var inactive = new Employee { Id = 3, FullName = "Dr. Inactive", Title = "None", IsActive = false };
                db.Add(inactive);
            }

            await db.SaveChangesAsync();
        }
    }
}
