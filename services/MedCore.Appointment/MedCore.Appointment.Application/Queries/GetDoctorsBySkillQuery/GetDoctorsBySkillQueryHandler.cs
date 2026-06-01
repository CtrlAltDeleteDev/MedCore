using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery
{
    public class GetDoctorsBySkillQueryHandler(AppoitmentDbContext appoitmentDbContext, ILogger<GetDoctorsBySkillQueryHandler> logger) : IRequestHandler<GetDoctorsBySkillQuery, Result<EmployeeDto[]>>
    {
        private readonly AppoitmentDbContext _appoitmentDbContext = appoitmentDbContext;
        private readonly ILogger<GetDoctorsBySkillQueryHandler> _logger = logger;

        public async Task<Result<EmployeeDto[]>> Handle(GetDoctorsBySkillQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var employees = await _appoitmentDbContext.Employees.Where(x => x.Skills.Any(s => request.SkillIds.Contains(s.Id)))
                    .Select(x => new EmployeeDto(x.Id, x.FullName, x.Title, x.Skills.Select(s => s.Id).ToArray()))
                    .ToArrayAsync(cancellationToken);

                return Result<EmployeeDto[]>.Ok(employees);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occurred while fetching doctors by skill.");
                return Result<EmployeeDto[]>.Fail("An error occurred while fetching doctors by skill.");
            }
        }
    }
}
