using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.Queries.GetDoctorQuery
{
    public record GetDoctorMeetsQuery(int doctorId) : IRequest<Result<MeetsShortDto[]>>
    {

    }
}
