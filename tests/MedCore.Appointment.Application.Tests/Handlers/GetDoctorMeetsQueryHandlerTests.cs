using MedCore.Appointment.Application.Queries.GetDoctorQuery;
using MedCore.Appointment.Application.Tests.Helpers;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Infrastructure.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace MedCore.Appointment.Application.Tests.Handlers
{
    public class GetDoctorMeetsQueryHandlerTests
    {
        [Fact]
        public async Task Returns_only_active_meets_for_doctor()
        {
            const string db = "meets_active_only";
            await SeedAsync(db);

            var result = await BuildHandler(db).Handle(new GetDoctorMeetsQuery(1), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Length); // "Cancelled" excluded
        }

        [Fact]
        public async Task Does_not_return_other_doctors_meets()
        {
            const string db = "meets_no_cross";
            await SeedAsync(db);

            var result = await BuildHandler(db).Handle(new GetDoctorMeetsQuery(2), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
        }

        [Fact]
        public async Task Results_are_ordered_by_start_time()
        {
            const string db = "meets_ordered";
            await SeedAsync(db);

            var result = await BuildHandler(db).Handle(new GetDoctorMeetsQuery(1), CancellationToken.None);

            var times = result.Value!.Select(m => m.StartTime).ToArray();
            Assert.Equal(times.OrderBy(t => t), times);
        }

        [Fact]
        public async Task Returns_empty_for_unknown_doctor()
        {
            const string db = "meets_unknown_doc";
            await SeedAsync(db);

            var result = await BuildHandler(db).Handle(new GetDoctorMeetsQuery(999), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        private static GetDoctorMeetsQueryHandler BuildHandler(string dbName)
        {
            var db = DbContextFactory.Create(dbName);
            return new(new MeetRepository(db), NullLogger<GetDoctorMeetsQueryHandler>.Instance);
        }

        private static async Task SeedAsync(string dbName)
        {
            await using var db = DbContextFactory.Create(dbName);
            var @base = DateTime.UtcNow.AddDays(1);

            db.AddRange(
            new Meet
            {
                EmployeeId = 1, PatientId = 1, Subject = "A", SkillIds = [1], IsActive = true,
                StartTime = @base, EndTime = @base.AddHours(1)
            },
            new Meet
            {
                EmployeeId = 1, PatientId = 2, Subject = "B", SkillIds = [1], IsActive = true,
                StartTime = @base.AddHours(2), EndTime = @base.AddHours(3)
            },
            new Meet
            {
                EmployeeId = 1, PatientId = 3, Subject = "Cancelled", SkillIds = [1], IsActive = false,
                StartTime = @base.AddHours(4), EndTime = @base.AddHours(5)
            },
            new Meet
            {
                EmployeeId = 2, PatientId = 4, Subject = "Other doc", SkillIds = [1], IsActive = true,
                StartTime = @base, EndTime = @base.AddHours(1)
            });
            await db.SaveChangesAsync();
        }
    }
}
