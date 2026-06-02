using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery
{
    public record GetDoctorsBySkillQuery(int[] SkillIds) : IRequest<Result<EmployeeDto[]>>
    {
    }
}
