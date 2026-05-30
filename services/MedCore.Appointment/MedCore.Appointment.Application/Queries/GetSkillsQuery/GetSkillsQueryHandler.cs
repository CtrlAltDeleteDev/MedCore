using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace MedCore.Appointment.Application.Queries.GetSkillsQuery
{
    public class GetSkillsQueryHandler(AppoitmentDbContext appoitmentDbContext, ILogger<GetSkillsQueryHandler> logger) : IRequestHandler<GetSkillsQuery, Result<SkillsDto>>
    {
        private readonly AppoitmentDbContext _appoitmentDbContext = appoitmentDbContext;
        private readonly ILogger<GetSkillsQueryHandler> _logger = logger;

        public async Task<Result<SkillsDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var skills = await _appoitmentDbContext
                    .Skills
                    .Select(x => new SkillDto(x.Id, x.Name, x.Description))
                    .ToArrayAsync(cancellationToken);

                return Result<SkillsDto>.Ok(new SkillsDto(skills));
            }
            catch(Exception ex) 
            {
                _logger.LogError("Error while fetching skills: {Message}", ex.Message);
                return Result<SkillsDto>.Fail("Error while fetching skills");
            }
        }
    }
}
