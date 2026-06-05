using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Appoitment.Data.Repositories;

public class EmployeeRepository(AppoitmentDbContext dbContext) : IRepository<Employee>
{
    private readonly AppoitmentDbContext _dbContext = dbContext;
    
    public async Task<IEnumerable<Employee>> GetItemsAsync(ISpecification<Employee> spec)
    {
        return await _dbContext.Employees.Where(spec.Criteria).ToListAsync();
    }
}