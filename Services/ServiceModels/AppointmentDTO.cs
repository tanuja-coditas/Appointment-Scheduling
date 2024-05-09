using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repo;


namespace Services.ServiceModels
{
    
    public class AppointmentDTO
    {
        public Guid AppointmentId { get; set; }
        public DateTime AppointmentsDateTime { get; set; }
        public string DoctorName { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public AppointmentDTO(Guid appointmentId,string doctorName, DateTime appointmentDatetIime , string status, string notes)
        {
            AppointmentId=appointmentId;
            DoctorName = doctorName;
            AppointmentsDateTime = appointmentDatetIime;
            Status = status;
            Notes = notes;
        }
    }
}
