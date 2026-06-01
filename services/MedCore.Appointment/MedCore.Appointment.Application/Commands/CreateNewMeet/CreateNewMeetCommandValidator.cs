using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.Commands.CreateNewMeet
{
    public class CreateNewMeetCommandValidator :AbstractValidator<CreateNewMeetCommand>
    {
        public CreateNewMeetCommandValidator()
        {
            RuleFor(x=>x.DocId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0)
                .WithMessage("DocId must be a positive integer.");

            RuleFor(x => x.PatientId)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0)
                .WithMessage("PatientId must be a positive integer.");

            RuleFor(x => x.StartDateTime)
                .NotNull()
                .NotEqual(DateTime.MinValue)
                .GreaterThan(DateTime.UtcNow).WithMessage("StartDateTime must be in the future.");

            RuleFor(x => x.EndDateTime)
                .NotNull()
                .NotEqual(DateTime.MinValue)
                .GreaterThan(x => x.StartDateTime).WithMessage("EndDateTime must be after StartDateTime.");

            RuleFor(x => x.SkillIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("SkillIds cannot be null or empty.");
        }
    }
}
