using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AuthServices;
using Services.ServiceModels;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppointmentScheduling.Controllers
{
    public class DoctorController : Controller
    {
        private readonly DoctorServices _doctorServices;

        // GET: DoctorController
        public DoctorController(DoctorServices doctorServices)
        {
            _doctorServices = doctorServices;
        }

        [Authorize(Roles ="doctor")]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Route("/Doctor/GetDoctors")]
        public IActionResult GetDoctors()
        {
            try
            {
                var doctors = _doctorServices.GetDoctors()
                                .Select(user => new {
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    PhoneNumber = user.PhoneNumber,
                                    Address = user.Address,
                                    ImagePath = user.UserImage,
                                    Email = user.Email,
                                    Specialization = user.Specialization,
                                }).ToArray();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                  
                };
                var json = JsonSerializer.Serialize(doctors, options);
                return Ok(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching doctors: {ex.Message}");
                return StatusCode(500); 
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchByName(string doctor)
        {
            var doctors = _doctorServices.GetDoctorsByName(doctor);
            TempData["SearchType"] = "name";
            TempData["SearchParameter"] = doctor;
            return View("Search",doctors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchBySpecialization(string specialization , string location)
        {
            var doctors = _doctorServices.GetDoctorsBySpecialization(specialization , location);
            TempData["SearchType"] = "specialization";
            TempData["SearchParameter"] = specialization+", "+location;
            return View("Search", doctors);
        }

        public IActionResult GetDoctorAvailability(string email)
        {

            var doctorsAvailability = _doctorServices.GetAvailability( email).ToArray();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(doctorsAvailability, options);
            return Ok(json);
        }
        public IActionResult GetAppointmentsForWeek()
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;
            var appointments = _doctorServices.GetAppointmentsForWeek(username).ToArray();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(appointments, options);
            return Ok(json);
        }

        public IActionResult GetTodaysAvailabilityAndAppointments()
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;
            var result = _doctorServices.GetTodaysAvailabilityAndAppointments(username).ToArray();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(result, options);
            return Ok(json);
   
        }

        public IActionResult GetWeeklyBreakdown()
        {
            string jwtToken = HttpContext.Request.Cookies["JWTToken"];
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
            Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");
            var username = usernameClaim.Value;

            var appointments = _doctorServices.GetWeeklyBreakdown(username);

            var groupedData = appointments.GroupBy(appointment => appointment.AppointmentDatetime.Date)
                                           .Select(group => new
                                           {
                                               DateTime = group.Key,
                                               Statuses = group.Select(g => g.AppointmentStatus ).ToList()
                                           }).ToArray();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            var json = JsonSerializer.Serialize(groupedData, options);
            return Ok(json);
            
        }

        public IActionResult Availability()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Availability(AvailabilityDTO availability)
        {
            if(ModelState.IsValid)
            {
                string jwtToken = HttpContext.Request.Cookies["JWTToken"];
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(jwtToken);
                Claim usernameClaim = parsedToken.Claims.FirstOrDefault(c => c.Type == "username");
                var username = usernameClaim.Value;

                await _doctorServices.AddAvailability(availability,username);
                return View();
            }
            return View(availability);
        }

        [HttpDelete]
        public IActionResult DeleteAvailability(Guid availabilityId)
        {
            _doctorServices.DeleteAvailability(availabilityId);
            return Ok();
        }
    }
}
