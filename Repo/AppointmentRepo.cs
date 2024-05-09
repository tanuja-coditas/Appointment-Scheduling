using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repo
{

    public class AppointmentRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public AppointmentRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public async Task Create(Guid patientId, Guid doctorId, DateTime appointmenDateTime)
        {
            var appointment = new TblAppointment()
            {
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmentStatus = "scheduled",
                Notes = "",
                AppointmentDatetime = appointmenDateTime
            };
            await _context.TblAppointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public List<TblAppointment> GetPatientAppointments(Guid patientID)
        {
            var appointments = _context.TblAppointments.Where(appointment => appointment.PatientId == patientID).ToList();
            return appointments;
        }

        public TblAppointment? GetAppointment(Guid appointmentId)
        {
            return _context.TblAppointments.FirstOrDefault(appointment => appointment.AppointmentId == appointmentId);
        }

        public async Task UpdateAppointment()
        {
           
            await _context.SaveChangesAsync();
        }
    }
}
