using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MediatR;

namespace MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery
{
    /// <summary>
    /// Returns all active doctors together with their assigned skill IDs.
    /// Skill-based filtering is performed client-side on the returned <see cref="EmployeeDto.SkillIds"/> collection.
    /// </summary>
    public record GetActiveDoctorsQuery() : IRequest<Result<EmployeeDto[]>>
    {
    }
}
