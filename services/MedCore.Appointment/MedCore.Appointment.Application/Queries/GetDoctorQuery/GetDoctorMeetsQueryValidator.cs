using FluentValidation;

namespace MedCore.Appointment.Application.Queries.GetDoctorQuery
{
    public class GetDoctorMeetsQueryValidator : AbstractValidator<GetDoctorMeetsQuery>
    {
        public GetDoctorMeetsQueryValidator()
        {
            RuleFor(x => x.DoctorId).NotEmpty().WithMessage("DoctorId cannot be empty.");
        }
    }
}
