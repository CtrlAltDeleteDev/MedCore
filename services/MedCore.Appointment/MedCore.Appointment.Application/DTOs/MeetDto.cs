using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.DTOs
{
    public record MeetDto(int id, int employeeId, int patientId, DateTime startTime, DateTime endTime);
}
