using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MedCore.Appointment.Application.Queries.GetSkillsQuery
{
    public class GetSkillsQueryHandler(IRepository<Skill> skillRepository, ILogger<GetSkillsQueryHandler> logger)
        : IRequestHandler<GetSkillsQuery, Result<SkillDto[]>>
    {
        private readonly IRepository<Skill> _skillRepository = skillRepository;
        private readonly ILogger<GetSkillsQueryHandler> _logger = logger;

        public async Task<Result<SkillDto[]>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var skills = await _skillRepository.GetItemsAsync(new GetAllSkillsSpec(), useAsNoTracking: true, cancellationToken);
                return Result<SkillDto[]>.Ok(skills.Select(x => new SkillDto(x.Id, x.Name, x.Description)).ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching skills: {Message}", ex.Message);
                return Result<SkillDto[]>.Fail("Error while fetching skills");
            }
        }
    }
}
