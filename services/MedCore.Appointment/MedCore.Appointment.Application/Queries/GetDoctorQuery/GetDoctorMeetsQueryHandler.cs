using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedCore.Appointment.Application.Queries.GetDoctorQuery
{
    public class GetDoctorMeetsQueryHandler : IRequestHandler<GetDoctorMeetsQuery, Result<MeetsShortDto[]>>
    {
        private readonly AppoitmentDbContext _appoitmentDbContext;
        private readonly ILogger<GetDoctorMeetsQueryHandler> _logger;

        public GetDoctorMeetsQueryHandler(AppoitmentDbContext appoitmentDbContext, ILogger<GetDoctorMeetsQueryHandler> logger)
        {
            _appoitmentDbContext = appoitmentDbContext;
            _logger = logger;
        }

        public async Task<Result<MeetsShortDto[]>> Handle(GetDoctorMeetsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _appoitmentDbContext.Meets
                    .Where(m => m.EmployeeId == request.DoctorId && m.IsActive)
                    .OrderBy(m => m.StartTime)
                    .Select(m => new MeetsShortDto(m.StartTime, m.EndTime))
                    .ToArrayAsync(cancellationToken);

                return Result<MeetsShortDto[]>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting doctor meets for doctor with id {DocId}", request.DoctorId);
                return Result<MeetsShortDto[]>.Fail("Error while getting doctor meets.");
            }
        }
    }
}
