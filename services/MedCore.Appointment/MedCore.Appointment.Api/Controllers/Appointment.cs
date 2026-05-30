using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Appointment.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class Appointment : ControllerBase
    {
        [HttpGet("my-appointments")]
        [Authorize]
        public async Task<IActionResult> GetMyAppointments()
        {
            var patientId = int.Parse(User.FindFirst("patient_id")!.Value);

            return Ok();
        }
    }
}
