using MedCore.Appointment.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.Commands.CreateNewMeet
{
    public record CreateNewMeetCommand(int DocId, DateTime StartDateTime, DateTime EndDateTime, int PatientId, int[] SkillIds) : IRequest<Result<Unit>>
    {
        public int PatientId { get; set; } = PatientId;
    }
}
