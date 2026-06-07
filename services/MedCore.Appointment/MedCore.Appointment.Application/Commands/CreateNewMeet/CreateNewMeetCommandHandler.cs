using MedCore.Appointment.Application.Common;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MedCore.Appointment.Application.Commands.CreateNewMeet
{
    public class CreateNewMeetCommandHandler(
        IRepository<Meet> meetRepository,
        IRepository<Skill> skillRepository,
        ILogger<CreateNewMeetCommandHandler> logger)
        : IRequestHandler<CreateNewMeetCommand, Result<Unit>>
    {
        private readonly IRepository<Meet> _meetRepository = meetRepository;
        private readonly IRepository<Skill> _skillRepository = skillRepository;
        private readonly ILogger<CreateNewMeetCommandHandler> _logger = logger;

        public async Task<Result<Unit>> Handle(CreateNewMeetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var docMeets = (await _meetRepository.GetItemsAsync(
                    new GetMeetsByEmployeeIdSpec(request.DocId), cancellationToken: cancellationToken)).ToList();

                if (!IsTimeSlotAvailable(request.StartDateTime, request.EndDateTime, docMeets))
                    return Result<Unit>.Fail("The selected time slot is not available for the doctor.");

                var skills = (await _skillRepository.GetItemsAsync(
                    new GetSkillsByIdsSpec(request.SkillIds), cancellationToken: cancellationToken));

                if (skills.Count() != request.SkillIds.Length)
                    return Result<Unit>.Fail("One or more selected skills are invalid.");

                var newSchedule = new Meet
                {
                    Subject = string.Join(", ", skills.Select(s => s.Name)),
                    StartTime = request.StartDateTime,
                    EndTime = request.EndDateTime,
                    EmployeeId = request.DocId,
                    PatientId = request.PatientId,
                    SkillIds = request.SkillIds,
                };

                await _meetRepository.Add(newSchedule, cancellationToken);
                return Result<Unit>.Ok(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new meet for doctor with id {DocId}", request.DocId);
                return Result<Unit>.Fail("An error occurred while creating the meet.");
            }
        }

        private bool IsTimeSlotAvailable(DateTime start, DateTime end, List<Meet> docMeets)
            => !docMeets.Any(meet => start < meet.EndTime && end > meet.StartTime);
    }
}
