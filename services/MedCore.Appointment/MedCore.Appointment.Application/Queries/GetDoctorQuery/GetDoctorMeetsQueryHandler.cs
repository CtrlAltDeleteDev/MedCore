using MedCore.Appointment.Application.Common;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appoitment.Data.Entities;
using MedCore.Appoitment.Data.Repositories;
using MedCore.Appoitment.Data.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MedCore.Appointment.Application.Queries.GetDoctorQuery
{
    public class GetDoctorMeetsQueryHandler(IRepository<Meet> meetRepository, ILogger<GetDoctorMeetsQueryHandler> logger)
        : IRequestHandler<GetDoctorMeetsQuery, Result<MeetsShortDto[]>>
    {
        private readonly IRepository<Meet> _meetRepository = meetRepository;
        private readonly ILogger<GetDoctorMeetsQueryHandler> _logger = logger;

        public async Task<Result<MeetsShortDto[]>> Handle(GetDoctorMeetsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var meets = await _meetRepository.GetItemsAsync(
                    new GetActiveMeetsByEmployeeIdSpec(request.doctorId), useAsNoTracking: true, cancellationToken);

                var result = meets
                    .OrderBy(m => m.StartTime)
                    .Select(m => new MeetsShortDto(m.StartTime, m.EndTime))
                    .ToArray();

                return Result<MeetsShortDto[]>.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting doctor meets for doctor with id {DocId}", request.doctorId);
                return Result<MeetsShortDto[]>.Fail("Error while getting doctor meets.");
            }
        }
    }
}
