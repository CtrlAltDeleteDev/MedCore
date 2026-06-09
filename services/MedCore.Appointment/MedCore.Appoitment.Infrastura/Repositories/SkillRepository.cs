using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Appoitment.Infrastructure.Repositories;

public class SkillRepository : IRepository<Skill>
{
    private readonly AppoitmentDbContext _dbContext;

    public SkillRepository(AppoitmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Skill>> GetItemsAsync(ISpecification<Skill> spec, bool useAsNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Skills
            .Where(spec.Criteria);

        return await (useAsNoTracking ? query.AsNoTracking() : query).ToListAsync(cancellationToken);
    }

    public Task Add(Skill entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Skills.Add(entity);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Skill> GetItemAsync(ISpecification<Skill> spec, bool useAsNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Skills;

        return await (useAsNoTracking ? query.AsNoTracking() : query).FirstOrDefaultAsync(spec.Criteria, cancellationToken);
    }

    public async Task Update(Skill entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}