using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery
{
    public class GetDoctorsBySkillQueryHandler : IRequestHandler<GetDoctorsBySkillQuery, Result<EmployeeDto[]>>
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ILogger<GetDoctorsBySkillQueryHandler> _logger;

        public GetDoctorsBySkillQueryHandler(IRepository<Employee> employeeRepository, ILogger<GetDoctorsBySkillQueryHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<Result<EmployeeDto[]>> Handle(GetDoctorsBySkillQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var employees = await _employeeRepository.GetItemsAsync(
                    new GetEmployeesBySkillIdsSpec(request.SkillIds), useAsNoTracking: true, cancellationToken);

                return Result<EmployeeDto[]>.Ok(
                    employees.Select(x => new EmployeeDto(x.Id, x.FullName, x.Title, x.Skills.Select(s => s.Id).ToArray())).ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching doctors by skill.");
                return Result<EmployeeDto[]>.Fail("An error occurred while fetching doctors by skill.");
            }
        }
    }
}
