using MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery;
using MedCore.Appointment.Application.Tests.Helpers;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Infrastructure.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace MedCore.Appointment.Application.Tests.Handlers
{
    public class GetDoctorsBySkillQueryHandlerTests
    {
        private static GetDoctorsBySkillQueryHandler BuildHandler(string dbName)
        {
            var db = DbContextFactory.Create(dbName);
            return new(new EmployeeRepository(db), NullLogger<GetDoctorsBySkillQueryHandler>.Instance);
        }

        private static async Task SeedAsync(string dbName)
        {
            await using var db = DbContextFactory.Create(dbName);

            var skillCaries   = new Skill { Id = 1, Name = "Caries Treatment", Description = "" };
            var skillCleaning = new Skill { Id = 2, Name = "Cleaning",         Description = "" };

            var doctor1 = new Employee { Id = 1, FullName = "Dr. A", Title = "Therapist", IsActive = true };
            doctor1.Skills.Add(skillCaries);
            doctor1.Skills.Add(skillCleaning);

            var doctor2 = new Employee { Id = 2, FullName = "Dr. B", Title = "Surgeon", IsActive = true };
            doctor2.Skills.Add(skillCaries);

            db.AddRange(doctor1, doctor2);
            await db.SaveChangesAsync();
        }

        [Fact]
        public async Task Returns_all_doctors_with_given_skill()
        {
            const string db = "get_by_skill_all";
            await SeedAsync(db);

            var result = await BuildHandler(db).Handle(
                new GetDoctorsBySkillQuery(new[] { 1 }), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Length);
        }

        [Fact]
        public async Task Returns_only_matching_doctor()
        {
            const string db = "get_by_skill_one";
            await SeedAsync(db);

            var result = await BuildHandler(db).Handle(
                new GetDoctorsBySkillQuery(new[] { 2 }), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.Equal(1, result.Value![0].Id);
        }

        [Fact]
        public async Task Returns_empty_when_no_match()
        {
            const string db = "get_by_skill_empty";
            await SeedAsync(db);

            var result = await BuildHandler(db).Handle(
                new GetDoctorsBySkillQuery(new[] { 99 }), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }
    }
}
