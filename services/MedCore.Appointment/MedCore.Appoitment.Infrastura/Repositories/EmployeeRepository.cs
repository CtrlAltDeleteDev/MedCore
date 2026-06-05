using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Appoitment.Infrastructure.Repositories;

public class EmployeeRepository(AppoitmentDbContext dbContext) : IRepository<Employee>
{
    private readonly AppoitmentDbContext _dbContext = dbContext;

    public async Task<IEnumerable<Employee>> GetItemsAsync(ISpecification<Employee> spec, bool useAsNoTracking = false,  CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Employees
            .Where(spec.Criteria);

        return await (useAsNoTracking ? query.AsNoTracking() : query).ToListAsync(cancellationToken);            
    }

    public async Task Add(Employee entity,  CancellationToken cancellationToken = default)
    {
        _dbContext.Employees.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(Employee entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Employee> GetItemAsync(ISpecification<Employee> spec, bool useAsNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Employees;

        return await (useAsNoTracking ? query.AsNoTracking() : query).FirstOrDefaultAsync(spec.Criteria, cancellationToken);
    }
}