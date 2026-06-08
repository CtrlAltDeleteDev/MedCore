using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedCore.Appointment.Application.Queries.GetSkillsQuery
{
    public class GetSkillsQueryHandler(AppoitmentDbContext appoitmentDbContext, ILogger<GetSkillsQueryHandler> logger) : IRequestHandler<GetSkillsQuery, Result<SkillDto[]>>
    {
        private readonly AppoitmentDbContext _appoitmentDbContext = appoitmentDbContext;
        private readonly ILogger<GetSkillsQueryHandler> _logger = logger;

        public async Task<Result<SkillDto[]>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var skills = await _appoitmentDbContext
                    .Skills
                    .Select(x => new SkillDto(x.Id, x.Name, x.Description))
                    .ToArrayAsync(cancellationToken);

                return Result<SkillDto[]>.Ok(skills);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching skills: {Message}", ex.Message);
                return Result<SkillDto[]>.Fail("Error while fetching skills");
            }
        }
    }
}
