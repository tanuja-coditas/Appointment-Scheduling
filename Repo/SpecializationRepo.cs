using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo
{
    public class SpecializationRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public SpecializationRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }

        public List<TblSpecialization> GetSepcialiazations()
        {
            var specializations = _context.TblSpecializations.ToList();
            return specializations;
        }

        public List<TblDoctorSpecialization> GetDoctorSpecialization()
        {
            return _context.TblDoctorSpecializations.ToList();
        }
    }
}
