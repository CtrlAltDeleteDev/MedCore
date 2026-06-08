using MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery;

namespace MedCore.Appointment.Application.Tests.Validators
{
    public class GetDoctorsBySkillQueryValidatorTests
    {
        private readonly GetDoctorsBySkillQueryValidator _sut = new();

        [Fact]
        public void Valid_skillIds_passes()
        {
            var result = _sut.Validate(new GetDoctorsBySkillQuery(new[] { 1, 2 }));
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Empty_skillIds_fails()
        {
            var result = _sut.Validate(new GetDoctorsBySkillQuery(Array.Empty<int>()));
            Assert.False(result.IsValid);
        }
    }
}
