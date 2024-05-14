using common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Services.AuthServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AppointmentScheduling.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientServices patientServices;

        public PatientController(PatientServices patientServices)
        {
            this.patientServices = patientServices;
        }
        [NoCache]
        [Authorize(Roles ="patient")]
        public IActionResult Index()
        {
            
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="patient")]
        public async Task<IActionResult> BookAppointment(Guid availabilityId,string doctorEmail)
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");
            await patientServices.BookAppointment(usernameClaim.Value,doctorEmail,availabilityId);
            return Ok();
        }

     
        [Authorize(Roles = "patient")]
        public IActionResult Appointments(string status="all")
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;
            var appointments = patientServices.GetAppointments(username);
            appointments = appointments.Where(appointment => appointment.Status.ToLower() == status.ToLower() || status.ToLower() == "all").ToList();
            return View("Appointments",appointments);
        }

        [HttpGet]
        public IActionResult ViewAppointment(Guid appointmentId)
        {
            var appointmentDetails = patientServices.ViewAppointment(appointmentId);
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(appointmentDetails, options);
            return Ok(json);
        }

        [HttpPut]
        [Authorize(Roles ="patient,doctor")]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            await patientServices.UpdateAppointmentStatus(appointmentId,"cancelled");
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "patient,doctor")]
        public async Task<IActionResult> CompleteAppointment(Guid appointmentId)
        {
            await patientServices.UpdateAppointmentStatus(appointmentId,"completed");
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> AddAppointmentNotes(Guid appointmentId,string notes)
        {
            var updatednotes = await patientServices.UpdateAppointmentNotes(appointmentId, notes);
            return Ok(updatednotes);
        }
    }
}
