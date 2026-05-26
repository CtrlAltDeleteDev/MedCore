using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Appointment.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class Appointment : ControllerBase
    {
        [HttpGet]
        [Route("qwe")]
        public IActionResult Getqwe()
        {
            return Ok("qwe");
        }
    }
}
