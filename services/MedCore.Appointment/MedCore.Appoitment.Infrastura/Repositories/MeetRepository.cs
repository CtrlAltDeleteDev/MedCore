using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Appoitment.Infrastura.Repositories;

public class MeetRepository(AppoitmentDbContext dbContext): IRepository<Meet, MeetDto>
{
    private readonly AppoitmentDbContext _dbContext = dbContext;
    public async Task<IEnumerable<MeetDto>> GetItemsAsync(ISpecification<Meet> spec, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Meets
            .Where(spec.Criteria)
            .Select(x => new MeetDto(x.Id, x.Subject, x.StartTime, x.EndTime, x.EmployeeId, x.PatientId, x.SkillIds))
            .ToListAsync(cancellationToken);

    }

    public async Task Add(Meet entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Meets.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}