using FluentValidation;

namespace MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery
{
    public class GetDoctorsBySkillQueryValidator : AbstractValidator<GetDoctorsBySkillQuery>
    {
        public GetDoctorsBySkillQueryValidator()
        {
            RuleFor(x => x.SkillIds).NotNull().NotEmpty().WithMessage("SkillIds cannot be null or empty.");
        }
    }
}
