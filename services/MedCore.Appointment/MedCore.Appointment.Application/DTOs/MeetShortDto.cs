using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.DTOs
{
    public record MeetsShortDto(DateTime startTime, DateTime? endTime);
}
