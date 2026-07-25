using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery
{
    public class GetActiveDoctorsQueryHandler : IRequestHandler<GetActiveDoctorsQuery, Result<EmployeeDto[]>>
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ILogger<GetActiveDoctorsQueryHandler> _logger;

        public GetActiveDoctorsQueryHandler(IRepository<Employee> employeeRepository, ILogger<GetActiveDoctorsQueryHandler> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<Result<EmployeeDto[]>> Handle(GetActiveDoctorsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var employees = await _employeeRepository.GetItemsAsync(
                    new GetActiveEmployeesSpec(), useAsNoTracking: true, cancellationToken);

                return Result<EmployeeDto[]>.Ok(
                    employees.Select(x => new EmployeeDto(x.Id, x.FullName, x.Title, x.Skills.Select(s => s.Id).ToArray())).ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching doctors.");
                return Result<EmployeeDto[]>.Fail("An error occurred while fetching doctors.");
            }
        }
    }
}
