using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Appoitment.Infrastructure.Repositories;

public class EmployeeRepository : IRepository<Employee>
{
    private readonly AppointmentDbContext _dbContext;

    public EmployeeRepository(AppointmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Employee>> GetItemsAsync(ISpecification<Employee> spec, bool useAsNoTracking = false,  CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Employees
            .Include(e => e.Skills)
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