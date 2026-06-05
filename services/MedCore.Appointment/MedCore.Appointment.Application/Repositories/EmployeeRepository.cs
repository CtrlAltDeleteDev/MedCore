using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Appointment.Application.Repositories;

public class EmployeeRepository(AppoitmentDbContext dbContext) : IRepository<Employee,EmployeeDto>
{
    private readonly AppoitmentDbContext _dbContext = dbContext;

    public async Task<IEnumerable<EmployeeDto>> GetItemsAsync(ISpecification<Employee> spec,
        bool useAsNoTracking = true)
    {
        var query = _dbContext.Employees
            .Where(spec.Criteria)
            .Select((x => new EmployeeDto(x.Id, x.FullName, x.Title, x.Skills.Select(s => s.Id).ToArray())));

        return await (useAsNoTracking ? query.AsNoTracking() : query).ToListAsync();
    }
}