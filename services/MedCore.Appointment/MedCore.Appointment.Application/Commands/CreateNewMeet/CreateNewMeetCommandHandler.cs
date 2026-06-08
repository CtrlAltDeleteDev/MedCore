using MedCore.Appointment.Application.Common;
using MedCore.Appoitment.Data;
using MedCore.Appoitment.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedCore.Appointment.Application.Commands.CreateNewMeet
{
    public class CreateNewMeetCommandHandler : IRequestHandler<CreateNewMeetCommand, Result<Unit>>
    {
        private readonly AppoitmentDbContext _appoitmentDbContext;
        private readonly ILogger<CreateNewMeetCommandHandler> _logger;

        public CreateNewMeetCommandHandler(AppoitmentDbContext appoitmentDbContext, ILogger<CreateNewMeetCommandHandler> logger)
        {
            _appoitmentDbContext = appoitmentDbContext;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(CreateNewMeetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var docMeets = await _appoitmentDbContext.Meets.Where(m => m.EmployeeId == request.DocId).ToListAsync(cancellationToken);

                if (!IsTimeSlotAvailable(request.StartDateTime, request.EndDateTime, docMeets))
                {
                    return Result<Unit>.Fail("The selected time slot is not available for the doctor.");
                }

                var skills = await _appoitmentDbContext.Skills.Where(s => request.SkillIds.Contains(s.Id)).ToListAsync(cancellationToken);

                if (skills.Count != request.SkillIds.Length)
                {
                    return Result<Unit>.Fail("One or more selected skills are invalid.");
                }

                var newSchedule = new Meet()
                {
                    Subject = string.Join(", ", skills.Select(s => s.Name)),
                    StartTime = request.StartDateTime,
                    EndTime = request.EndDateTime,
                    EmployeeId = request.DocId,
                    PatientId = request.PatientId,
                    SkillIds = request.SkillIds,
                };

                _appoitmentDbContext.Meets.Add(newSchedule);

                await _appoitmentDbContext.SaveChangesAsync(cancellationToken);

                return Result<Unit>.Ok(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating new meet for doctor with id {DocId}", request.DocId);
                return Result<Unit>.Fail("An error occurred while creating the meet.");
            }
        }

        private bool IsTimeSlotAvailable(DateTime start, DateTime end, List<Meet> docMeets)
        {
            return !docMeets.Any(meet => start < meet.EndTime && end > meet.StartTime);
        }
    }
}
