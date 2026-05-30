using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MediatR;

namespace MedCore.Appointment.Application.Queries.GetSkillsQuery
{
    public class GetSkillsQuery : IRequest<Result<SkillsDto>>
    {
    }
}
