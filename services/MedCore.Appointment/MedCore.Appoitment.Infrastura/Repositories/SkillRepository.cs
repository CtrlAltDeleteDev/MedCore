using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Appoitment.Infrastura.Repositories;

public class SkillRepository(AppoitmentDbContext dbContext):IRepository<Skill, SkillDto>
{
    private readonly AppoitmentDbContext _dbContext = dbContext;
    public async Task<IEnumerable<SkillDto>> GetItemsAsync(ISpecification<Skill> spec, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Skills
            .Where(spec.Criteria)
            .Select(x => new SkillDto(x.Id, x.Name, x.Description))
            .ToListAsync(cancellationToken);
    }

    public Task Add(Skill entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Skills.Add(entity);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}