using AppointmentAPI.Dtos;
using AppointmentAPI.Model;
using AppointmentAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppointmentAPI.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentsService;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(AppointmentService appointmentService, ILogger<AppointmentController> logger)
        {
            _appointmentsService = appointmentService;
            _logger = logger;
        }



        // POST: api/appointments
        [HttpPost("createappointment")]
        public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] Appointment appointmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create the Appointment object
            var appointment = new Appointment
            {
                PatientName = appointmentDto.PatientName,
                PatientId = appointmentDto.PatientId,
                PhoneNumber = appointmentDto.PhoneNumber,
                Date = appointmentDto.Date,
                Speciality = appointmentDto.Speciality,
                DoctorId = appointmentDto.DoctorId,
                SelectedSlot = appointmentDto.SelectedSlot,
                AvailabilityId = appointmentDto.AvailabilityId, // Ensure this is set correctly
            };

            // Call the service to create the appointment
            var createdAppointment = await _appointmentsService.CreateAppointment(appointment);

            if (createdAppointment == null)
            {
                return BadRequest("Failed to create appointment.");
            }

            return CreatedAtAction(nameof(GetAppointmentById), new { id = createdAppointment.AppointmentId }, createdAppointment);
        }


        // GET: api/appointments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(string id)
        {
            var appointment = await _appointmentsService.GetAppointmentById(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return appointment; // Assuming this is of type Appointment
        }

        // GET: api/appointments
        [HttpGet("getallappointment")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            var appointments = await _appointmentsService.GetAllAppointments();
            return Ok(appointments);
        }

        // PUT: api/appointments/{id}
        [HttpPut("updateappointment/{id}")]
        public async Task<ActionResult> UpdateAppointment(string id, [FromBody] Appointment appointment)
        {
            var result = await _appointmentsService.UpdateAppointment(id, appointment);
            if (!result)
            {
                return NotFound();
            }

            return NoContent(); // 204 No Content
        }

        // DELETE: api/appointments/{id}
        [HttpDelete("deleteappointment/{id}")]
        public async Task<ActionResult> DeleteAppointment(string id)
        {
            var result = await _appointmentsService.DeleteAppointment(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent(); // 204 No Content
        }

        [HttpGet("bookedtimes")]
        public async Task<ActionResult<IEnumerable<BookedAppointment>>> GetAllBookedAppointments()
        {
            var bookedAppointments = await _appointmentsService.GetAllBookedAppointments();

            if (bookedAppointments == null || !bookedAppointments.Any())
            {
                return NotFound("No booked appointments found.");
            }

            return Ok(bookedAppointments);
        }
    }

    // GET: api/appointments/booked
  

}
