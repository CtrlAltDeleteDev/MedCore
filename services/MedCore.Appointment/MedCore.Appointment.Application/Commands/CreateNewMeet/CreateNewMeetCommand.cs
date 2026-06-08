using MedCore.Appointment.Application.Common;
using MediatR;

namespace MedCore.Appointment.Application.Commands.CreateNewMeet
{
    public record CreateNewMeetCommand(int DocId, DateTime StartDateTime, DateTime EndDateTime, int PatientId, int[] SkillIds) : IRequest<Result<Unit>>
    {
        public int PatientId { get; set; } = PatientId;
    }
}
