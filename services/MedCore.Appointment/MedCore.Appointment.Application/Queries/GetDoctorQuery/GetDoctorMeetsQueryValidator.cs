using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.Queries.GetDoctorQuery
{
    public class GetDoctorMeetsQueryValidator : AbstractValidator<GetDoctorMeetsQuery>
    {
        public GetDoctorMeetsQueryValidator()
        {
            RuleFor(x => x.doctorId).NotEmpty().WithMessage("DoctorId cannot be empty.");
        }
    }
}
