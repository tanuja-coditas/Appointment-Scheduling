using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.AuthServices;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

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
    }
}
