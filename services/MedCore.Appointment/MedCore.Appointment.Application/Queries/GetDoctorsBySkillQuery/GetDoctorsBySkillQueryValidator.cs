using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery
{
    public class GetDoctorsBySkillQueryValidator: AbstractValidator<GetDoctorsBySkillQuery>
    {
        public GetDoctorsBySkillQueryValidator()
        {
            RuleFor(x => x.SkillIds).NotNull().NotEmpty().WithMessage("SkillIds cannot be null or empty.");
        }
    }
}
