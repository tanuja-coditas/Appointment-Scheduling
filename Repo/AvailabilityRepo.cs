using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class AvailabilityRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public AvailabilityRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public List<TblAvailability> GetAvailability(Guid doctorId)
        {
            return _context.TblAvailabilities.Where(availability => availability.DoctorId == doctorId).ToList();
        }

        public DateTime UpdateAvailability(Guid availabilityId)
        {
           var availability =_context.TblAvailabilities.FirstOrDefault(availability => availability.AvailabilityId == availabilityId);
            if(availability !=null)
            {
                availability.AppointmentCount--;
                _context.SaveChanges();
                var datetime = availability.AvailabilityStartDatetime;
                if (availability.AppointmentCount == 0)
                {
                   DeleteAvailability(availability.AvailabilityId);
                }
                return datetime;
            }
            return default(DateTime);
        }

        public void DeleteAvailability(Guid availabilityId)
        {
            var availability = _context.TblAvailabilities.FirstOrDefault(availability=> availability.AvailabilityId==availabilityId);

            if (availability != null)
            {
             
                _context.TblAvailabilities.Remove(availability);
                _context.SaveChanges();
            }
        }
    }
}
