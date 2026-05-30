using MedCore.Appointment.Application.Queries.GetSkillsQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Appointment.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class Appointment(IMediator mediator ) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("skills")]
        public async Task<IActionResult> GetSkillsAsync() 
        {
            var query = new GetSkillsQuery();

            var result = await _mediator.Send(query);

            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }
    }
}
