using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.DTOs
{
    public record EmployeeDto(int Id, string FullName, string Title, int[] SkillIds);
}
