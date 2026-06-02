using MedCore.Appointment.Application.Commands.CreateNewMeet;

namespace MedCore.Appointment.Application.Tests.Validators
{
    public class CreateNewMeetCommandValidatorTests
    {
        private readonly CreateNewMeetCommandValidator _sut = new();

        private static CreateNewMeetCommand ValidCommand() => new(
            DocId: 1,
            StartDateTime: DateTime.UtcNow.AddDays(1),
            EndDateTime: DateTime.UtcNow.AddDays(1).AddHours(1),
            PatientId: 1,
            SkillIds: new[] { 1 });

        [Fact]
        public void Valid_command_passes()
        {
            var result = _sut.Validate(ValidCommand());
            Assert.True(result.IsValid);
        }

        [Fact]
        public void DocId_zero_fails()
        {
            var cmd = ValidCommand() with { DocId = 0 };
            var result = _sut.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.DocId));
        }

        [Fact]
        public void PatientId_zero_fails()
        {
            var cmd = ValidCommand() with { PatientId = 0 };
            var result = _sut.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.PatientId));
        }

        [Fact]
        public void StartDateTime_in_past_fails()
        {
            var cmd = ValidCommand() with { StartDateTime = DateTime.UtcNow.AddHours(-1) };
            var result = _sut.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.StartDateTime));
        }

        [Fact]
        public void EndDateTime_before_start_fails()
        {
            var start = DateTime.UtcNow.AddDays(1);
            var cmd = ValidCommand() with { StartDateTime = start, EndDateTime = start.AddMinutes(-30) };
            var result = _sut.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.EndDateTime));
        }

        [Fact]
        public void Empty_skillIds_fails()
        {
            var cmd = ValidCommand() with { SkillIds = Array.Empty<int>() };
            var result = _sut.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(cmd.SkillIds));
        }
    }
}
