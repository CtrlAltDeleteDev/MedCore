using FluentValidation;
using MedCore.Appointment.Application.Commands.CreateNewMeet;
using MedCore.Appointment.Application.DTOs;
using MedCore.Appointment.Application.Queries.GetDoctorQuery;
using MedCore.Appointment.Application.Queries.GetDoctorsBySkillQuery;
using MedCore.Appointment.Application.Queries.GetSkillsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Appointment.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    [Produces("application/json")]
    public class Appointment(IMediator mediator,
        IValidator<GetDoctorsBySkillQuery> getDocBySkillsValidator,
        IValidator<GetDoctorMeetsQuery> getDocMeetsValidator,
        IValidator<CreateNewMeetCommand> createNewMeetValidator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IValidator<GetDoctorsBySkillQuery> _getDocBySkillsValidator = getDocBySkillsValidator;
        private readonly IValidator<GetDoctorMeetsQuery> _getDocMeetsValidator = getDocMeetsValidator;
        private readonly IValidator<CreateNewMeetCommand> _createNewMeetValidator = createNewMeetValidator;

        /// <summary>Returns all available dental skills / procedures.</summary>
        /// <response code="200">List of skills.</response>
        /// <response code="400">Internal error while fetching skills.</response>
        [HttpGet("skills")]
        [ProducesResponseType(typeof(SkillDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSkillsAsync()
        {
            var result = await _mediator.Send(new GetSkillsQuery());
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>Returns doctors who have all of the specified skills.</summary>
        /// <param name="skillIds">One or more skill IDs to filter by.</param>
        /// <response code="200">Matching doctors with their skill IDs.</response>
        /// <response code="400">Validation error or internal error.</response>
        [HttpGet("doctors-by-skills")]
        [ProducesResponseType(typeof(EmployeeDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDoctorsBySkillAsync([FromQuery] int[] skillIds)
        {
            var query = new GetDoctorsBySkillQuery(skillIds);
            var validationResult = _getDocBySkillsValidator.Validate(query);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>Returns the active appointment calendar for a specific doctor.</summary>
        /// <param name="doctorId">Doctor (employee) ID.</param>
        /// <response code="200">List of appointments ordered by start time.</response>
        /// <response code="400">Validation error or internal error.</response>
        [HttpGet("doctor-calendar")]
        [ProducesResponseType(typeof(MeetsShortDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDoctorCalendar([FromQuery] int doctorId)
        {
            var query = new GetDoctorMeetsQuery(doctorId);
            var validationResult = _getDocMeetsValidator.Validate(query);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>Books a new appointment for the authenticated patient.</summary>
        /// <remarks>PatientId is extracted automatically from the JWT <c>patient_id</c> claim.</remarks>
        /// <response code="201">Appointment created successfully.</response>
        /// <response code="400">Validation error, time slot conflict, or invalid skills.</response>
        /// <response code="401">Missing or invalid JWT token.</response>
        [HttpPost("schedule-meet")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateNewMeet([FromBody] CreateNewMeetCommand command)
        {
            command.PatientId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "patient_id")?.Value ?? "0");

            var validationResult = _createNewMeetValidator.Validate(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Created() : BadRequest(result.Error);
        }
    }
}
