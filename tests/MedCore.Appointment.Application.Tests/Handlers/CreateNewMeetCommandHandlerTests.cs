using MedCore.Appointment.Application.Commands.CreateNewMeet;
using MedCore.Appointment.Application.Tests.Helpers;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Infrastructure.Repositories;
using Microsoft.Extensions.Logging.Abstractions;

namespace MedCore.Appointment.Application.Tests.Handlers
{
    public class CreateNewMeetCommandHandlerTests
    {
        // --- tests ---
        [Fact]
        public async Task Creates_meet_when_slot_is_free()
        {
            var db = $"creates_meet_free_{Guid.NewGuid()}";
            await SeedAsync(db, docId: 1, skillId: 1);

            // verify seed is visible in a second context
            await using var verifyCtx = DbContextFactory.Create(db);
            var seededSkills = verifyCtx.Set<Skill>().Count();
            Assert.Equal(1, seededSkills);   // diagnostic: seed must have written the skill

            var handler = BuildHandler(db);
            var result = await handler.Handle(BaseCommand(1, 1), CancellationToken.None);

            Assert.True(result.IsSuccess, result.Error);
        }

        [Fact]
        public async Task Fails_when_time_slot_overlaps()
        {
            const string db = "fails_overlap";
            var start = DateTime.UtcNow.AddDays(1);
            await SeedWithMeetAsync(db, docId: 1, skillId: 1, start, start.AddHours(2));
            var handler = BuildHandler(db);

            // overlapping meet: starts 30 min into existing
            var cmd = new CreateNewMeetCommand(
                DocId: 1,
                StartDateTime: start.AddMinutes(30),
                EndDateTime: start.AddMinutes(90),
                PatientId: 5,
                SkillIds: new[] { 1 });

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Contains("not available", result.Error);
        }

        [Fact]
        public async Task Fails_when_skill_id_is_invalid()
        {
            const string db = "fails_invalid_skill";
            await SeedAsync(db, docId: 1, skillId: 1);
            var handler = BuildHandler(db);

            var cmd = BaseCommand(docId: 1, skillId: 999); // non-existent skill

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Contains("invalid", result.Error);
        }

        [Fact]
        public async Task Sets_patientId_from_command()
        {
            const string db = "sets_patientid";
            await SeedAsync(db, docId: 1, skillId: 1);
            var handler = BuildHandler(db);

            var cmd = BaseCommand(docId: 1, skillId: 1) with { PatientId = 7 };
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            await using var db2 = DbContextFactory.Create(db);
            var meet = db2.Meets.First();
            Assert.Equal(7, meet.PatientId);
        }

        [Fact]
        public async Task Subject_is_built_from_skill_names()
        {
            const string db = "subject_from_skills";
            await SeedAsync(db, docId: 1, skillId: 1);
            var handler = BuildHandler(db);

            await handler.Handle(BaseCommand(1, 1), CancellationToken.None);

            await using var db2 = DbContextFactory.Create(db);
            var meet = db2.Meets.First();
            Assert.Equal("Test Skill", meet.Subject);
        }

        [Fact]
        public async Task Adjacent_slots_are_allowed()
        {
            const string db = "adjacent_slots";
            var start = DateTime.UtcNow.AddDays(1);
            await SeedWithMeetAsync(db, docId: 1, skillId: 1, start, start.AddHours(1));
            var handler = BuildHandler(db);

            // starts exactly when previous ends — should be OK
            var cmd = new CreateNewMeetCommand(
                DocId: 1,
                StartDateTime: start.AddHours(1),
                EndDateTime: start.AddHours(2),
                PatientId: 5,
                SkillIds: new[] { 1 });

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        private static CreateNewMeetCommandHandler BuildHandler(string dbName)
        {
            var db = DbContextFactory.Create(dbName);
            return new(new MeetRepository(db), new SkillRepository(db), NullLogger<CreateNewMeetCommandHandler>.Instance);
        }

        private static CreateNewMeetCommand BaseCommand(int docId, int skillId) => new(
        DocId: docId,
        StartDateTime: DateTime.UtcNow.AddDays(1),
        EndDateTime: DateTime.UtcNow.AddDays(1).AddHours(1),
        PatientId: 3,
        SkillIds: new[] { skillId });

        // --- helpers ---
        private static async Task SeedAsync(string dbName, int docId, int skillId)
        {
            await using var db = DbContextFactory.Create(dbName);
            db.Set<Skill>().Add(new Skill { Id = skillId, Name = "Test Skill", Description = "desc" });
            db.Set<Employee>().Add(new Employee { Id = docId, FullName = "Dr. Test", Title = "Doctor", IsActive = true });
            await db.SaveChangesAsync();
        }

        private static async Task SeedWithMeetAsync(
            string dbName,
            int docId,
            int skillId,
            DateTime start,
            DateTime end)
        {
            await using var db = DbContextFactory.Create(dbName);
            var skill = new Skill { Id = skillId, Name = "Test Skill", Description = "desc" };
            db.Add(skill);
            await db.SaveChangesAsync();

            db.Add(new Meet
            {
                EmployeeId = docId, PatientId = 99,
                Subject = "existing", SkillIds = new[] { skillId },
                StartTime = start, EndTime = end
            });
            await db.SaveChangesAsync();
        }
    }
}
