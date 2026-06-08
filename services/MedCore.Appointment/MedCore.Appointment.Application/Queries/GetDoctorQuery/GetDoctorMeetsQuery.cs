using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MediatR;

namespace MedCore.Appointment.Application.Queries.GetDoctorQuery
{
    public record GetDoctorMeetsQuery(int DoctorId) : IRequest<Result<MeetsShortDto[]>>
    {
    }
}
