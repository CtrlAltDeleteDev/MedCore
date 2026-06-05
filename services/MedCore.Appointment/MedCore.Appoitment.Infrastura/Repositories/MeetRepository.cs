using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Appoitment.Infrastructure.Repositories;

public class MeetRepository(AppoitmentDbContext dbContext): IRepository<Meet>
{
    private readonly AppoitmentDbContext _dbContext = dbContext;
    public async Task<IEnumerable<Meet>> GetItemsAsync(ISpecification<Meet> spec, bool useAsNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Meets
            .Where(spec.Criteria);
            
        return await (useAsNoTracking ? query.AsNoTracking() : query).ToListAsync(cancellationToken);
    }

    public async Task Add(Meet entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Meets.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async  Task<Meet> GetItemAsync(ISpecification<Meet> spec, bool useAsNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Meets;

        return await (useAsNoTracking ? query.AsNoTracking() : query).FirstOrDefaultAsync(spec.Criteria, cancellationToken);
    }

    public async Task Update(Meet entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}